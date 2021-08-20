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

    }

   public class MachineProfileModel : PageModel
    {
        public string chartlabels { get; set; }
        public string chartdataset { get; set; }
        public WorkOrder WO { get; set; }
        public List<InputedMaterial> Inputed { get; set; }

        private GlobalDatabase dbglob = new GlobalDatabase();

        private ChartDownTimeDataSet ChartDataScript;

        private List<string> labels;


        public void OnGet(string EQ_ID)
        {
            if (EQ_ID != null)
            {
                // Get downtimes
                DataTable dt = dbglob.GetNonWorkSum(EQ_ID);
                ChartDataScript = new ChartDownTimeDataSet();

                FillDataScript(dt);

                chartdataset = JsonConvert.SerializeObject(ChartDataScript, Formatting.Indented);
                chartlabels = JsonConvert.SerializeObject(labels, Formatting.Indented);

                // Get WO
                WO = new WorkOrder();
                WO.LoadFromMachine(EQ_ID);

                // Get inputed material
                LoadInputedMaterial(EQ_ID);    
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

            ChartDataScript.AddFirst(((12*60)-sumtime).ToString(), KnownColor.LimeGreen);
            labels.Insert(0,"Work");

        }

    }
}
