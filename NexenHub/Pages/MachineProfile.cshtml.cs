using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NexenHub.Class;
using NexenHub.Controllers;
using NexenHub.Models;

namespace NexenHub.Pages
{

    public class InputedMaterial
    {
        public string IO_POSID { get; set; }
        public string LOT_ID { get; set; }
        public string ITEM_ID { get; set; }
        public string ITEM_NAME { get; set; }
        public string CART_ID { get; set; }
        public string EQ_ID { get; set; }
        public Boolean IsInBom { get; set; }

    }

    public class MachineBasicInfo
    {
        public string Name { get; set; }
        public string EQ_ID { get; set; }
        public string WC_ID { get; set; }
        public string PROC_ID { get; set; }

        private GlobalDatabase db = new GlobalDatabase();

        public MachineBasicInfo(string EQID)
        {
            EQ_ID = EQID;
            DataTable dt = db.GetMachineList(EQ_ID);
            if (dt.Rows.Count > 0)
            {
                Name = dt.Rows[0]["Name"].ToString();
                WC_ID = dt.Rows[0]["WC_ID"].ToString();
                PROC_ID = dt.Rows[0]["PROC_ID"].ToString();
            }
        }
    }

    public class DownTimeInfoScript
    {
        public string id { get; set; }

        public string content { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string type => "background";
        public string className => "negative";
    }

    public class MachineProfileModel : PageModel
    {
        public string chartlabels { get; set; }
        public string chartdataset { get; set; }
        public WorkOrder WO { get; set; }
        public double QuantityPrc;
        public List<InputedMaterial> Inputed { get; set; }

        public List<InputedMaterial> BOM { get; set; }

        public MachineBasicInfo machineBasic { get; set; }

        public List<DownTimeInfoScript> downInfo { get; set; }
        public string downScript {get;set;}

        [BindProperty]
        public DateTime FilterDate { get; set; }

        [BindProperty]
        public bool IsA { get; set; }


        public string ActNonWork = "";

        private GlobalDatabase dbglob = new GlobalDatabase();
        private ChartDownTimeDataSet ChartDataScript;
        private List<string> labels;

        public void OnGet(string EQ_ID)
        {
            if (EQ_ID != null)
            {
                // Machine info
                machineBasic = new MachineBasicInfo(EQ_ID);

                // Get downtimes
                DataTable dt = dbglob.GetNonWorkSum(EQ_ID);
                ChartDataScript = new ChartDownTimeDataSet();
                FillDataScript(dt);

                // Get last nonworks
                FillDownTimeInfo(EQ_ID);

                // Act downtime
                dt = dbglob.GetActNonWrk(EQ_ID);
                if (dt.Rows.Count > 0)
                    ActNonWork = dt.Rows[0][0].ToString();

                chartdataset = JsonConvert.SerializeObject(ChartDataScript, Formatting.Indented);
                chartlabels = JsonConvert.SerializeObject(labels, Formatting.Indented);

                // Get WO
                WO = new WorkOrder();
                WO.LoadFromMachine(EQ_ID);

                if (string.IsNullOrEmpty(WO.PROD_QTY) || string.IsNullOrEmpty(WO.WO_QTY))
                    QuantityPrc = 0;
                else
                    QuantityPrc = Math.Round(((Double.Parse(WO.PROD_QTY) / Double.Parse(WO.WO_QTY)) * 100), 0);

                // BOM
                if (WO.TEST_YN == "Y") 
                    LoadTestBOM(WO.ITEM_ID, WO.PROTOTYPE_ID, WO.PROTOTYPE_VER);
                else
                    LoadBOM(WO.ITEM_ID);

                // Get inputed material
                LoadInputedMaterial(EQ_ID);

                

                SetFilterDate();
            }
        }

        public void FillDownTimeInfo(string EQ_ID)
        {
            downInfo = new List<DownTimeInfoScript>();
            DataTable dt = dbglob.GetLastNonWorkSum(EQ_ID, DateTime.Now.AddHours(-12));
            int i = 0;
            foreach (DataRow row in dt.Rows)
            {
                DownTimeInfoScript di = new DownTimeInfoScript();
                di.start = DateTime.Parse(row["STIME"].ToString()).ToString("yyyy-MM-ddTHH:mm:ss");
                di.end = !string.IsNullOrEmpty(row["ETIME"].ToString()) ? DateTime.Parse(row["ETIME"].ToString()).ToString("yyyy-MM-ddTHH:mm:ss") : null;
                di.id = "id_" + row["NON_NAME"].ToString() + "_"+i;
                di.content = row["NON_NAME"].ToString();
                downInfo.Add(di);
                i++;
            }
            downScript = JsonConvert.SerializeObject(downInfo, Formatting.Indented);
        }
        public void SetFilterDate()
        {
            DateTime now = DateTime.Now;
            IsA = now.Hour >= 6 && now.Hour < 18 ? true : false;

            FilterDate = now.AddHours(-6);
        }

        public void LoadTestBOM(string ITEM_ID,string PROTOTYPE_ID, string PROTOTYPE_VER)
        {
            BOM = new List<InputedMaterial>();
            DataTable dt = dbglob.GetPrototypeBOM(ITEM_ID, PROTOTYPE_ID, PROTOTYPE_VER);
            foreach (DataRow row in dt.Rows)
            {
                InputedMaterial material = new InputedMaterial();
                material.ITEM_ID = row["ITEM_ID"].ToString();
                material.ITEM_NAME = row["ITEM_NAME"].ToString();
                BOM.Add(material);
            }
        }

        public void LoadBOM(string ITEM_ID)
        {
            BOM = new List<InputedMaterial>();
            DataTable dt = dbglob.GetBOM(ITEM_ID);
            foreach (DataRow row in dt.Rows)
            {
                InputedMaterial material = new InputedMaterial();
                material.ITEM_ID = row["ITEM_ID"].ToString();
                material.ITEM_NAME = row["ITEM_NAME"].ToString();
                BOM.Add(material);
            }
        }

        public void LoadInputedMaterial(string EQ_ID)
        {
            Inputed = new List<InputedMaterial>();
            DataTable dt = dbglob.GetInputedMaterial(EQ_ID);
            foreach (DataRow row in dt.Rows)
            {
                InputedMaterial material = new InputedMaterial();
                material.EQ_ID = EQ_ID;
                material.LOT_ID = row["LOT_ID"].ToString();
                material.IO_POSID = row["IO_POSID"].ToString();
                material.ITEM_ID = row["ITEM_ID"].ToString();
                material.ITEM_NAME = row["ITEM_NAME"].ToString();
                material.CART_ID = row["CART_ID"].ToString();
                Inputed.Add(material);
            }
            
            foreach (InputedMaterial input in Inputed)
                input.IsInBom = BOM.Find(x => x.ITEM_ID == input.ITEM_ID) != null ? true : false;
        }

        public void FillDataScript(DataTable dt)
        {
            labels = new List<string>();

            float sumtime = 0;

            foreach (DataRow row in dt.Rows)
            {
                string stime = row[0].ToString();
                float time = float.Parse(stime);
                sumtime += time;
                ChartDataScript.Add(stime);
                labels.Add(row[1].ToString());
            }

            DateTime NowTime = DateTime.Now;
            int NowHour = NowTime.Hour;
            int Minutes = NowTime.Minute;

            // Number of minutes in this shift
            int FinalMinutes;

            if (NowHour >= 6 && NowHour < 18) 
                FinalMinutes = ((NowHour * 60) + Minutes) - (6 * 60);
            else
            {
                if (NowHour >= 0 && NowHour < 6)
                    FinalMinutes = ((NowHour * 60) + Minutes) + (6 * 60);
                else
                    FinalMinutes = ((NowHour * 60) + Minutes) - (18 * 60);
            }
            ChartDataScript.AddFirst((Math.Abs(FinalMinutes-sumtime)).ToString(), KnownColor.LimeGreen);
            labels.Insert(0,"Work");

        }

        public void OnPost(string EQ_ID)
        {
            OnGet(EQ_ID);
        }

   }
}
