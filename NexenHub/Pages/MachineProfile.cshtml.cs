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

    public class MachineProfileModel : PageModel
    {
        public MachineProdReport machineProduction { get; set; }
        public MachineDownTime machineDownTime { get; set; }
        public WorkOrder WO { get; set; }
        public List<InputedMaterial> Inputed { get; set; }
        public List<InputedMaterial> BOM { get; set; }
        public MachineBasicInfo machineBasic { get; set; }

        public string ActNonWork = "";

        private GlobalDatabase dbglob = new GlobalDatabase();

        public void OnGet(string EQ_ID)
        {
            if (EQ_ID != null)
            {
                // Machine info
                machineBasic = new MachineBasicInfo(EQ_ID);

                // Machine production - TODO when 1 day show hours
                machineProduction = new MachineProdReport(EQ_ID);

                // Get downtimes
                machineDownTime = new MachineDownTime(EQ_ID);

                // Act downtime
                DataTable dt = dbglob.GetActNonWrk(EQ_ID);
                if (dt.Rows.Count > 0)
                    ActNonWork = dt.Rows[0][0].ToString();

                // Get WO
                WO = new WorkOrder();
                WO.LoadFromMachine(EQ_ID);

                // BOM
                if (WO.TEST_YN == "Y") 
                    LoadTestBOM(WO.ITEM_ID, WO.PROTOTYPE_ID, WO.PROTOTYPE_VER);
                else
                    LoadBOM(WO.ITEM_ID);

                // Get inputed material
                LoadInputedMaterial(EQ_ID);
            }
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

   }
}
