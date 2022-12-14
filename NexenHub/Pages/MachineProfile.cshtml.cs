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
    public class MachineProfileModel : PageModel
    {
        public WorkOrder WO { get; set; }
        public List<InputedMaterial> Inputed { get; set; }
        public List<InputedMaterial> BOM { get; set; }
        public MachineBasicInfo machineBasic { get; set; }

        #region DownTime

        private string _DownTimeMessage;
        public string DownTimeMessage 
        { 
            get => _DownTimeMessage;
            set 
            {
                _DownTimeMessage = value;
                if (_DownTimeMessage == "PM")
                    IsPM = true;
                else
                    IsDowntime = true;
            }
        }

        public bool IsPM;
        public bool IsDowntime;

        #endregion

        private GlobalDatabase dbglob = new GlobalDatabase();

        public void OnGet(string EQ_ID)
        {
            if (EQ_ID != null)
            {
                // Machine info
                machineBasic = new MachineBasicInfo(EQ_ID);

                // Act downtime
                DataTable dt = dbglob.GetActNonWrk(EQ_ID);
                if (dt.Rows.Count > 0)
                    DownTimeMessage = dt.Rows[0][0].ToString();

                // Get WO
                WO = new WorkOrder();
                WO.LoadFromMachine(EQ_ID);

                // BOM
                if (WO.TEST_YN == "Y") 
                    LoadTestBOM(WO.ITEM_ID, WO.PROTOTYPE_ID, WO.PROTOTYPE_VER);
                else
                    LoadBOM(WO.ITEM_ID);

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

        //public void LoadInputedMaterial(string EQ_ID)
        //{
        //    Inputed = new List<InputedMaterial>();
        //    DataTable dt = dbglob.GetInputedMaterial(EQ_ID);
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        InputedMaterial material = new InputedMaterial();
        //        material.EQ_ID = EQ_ID;
        //        material.LOT_ID = row["LOT_ID"].ToString();
        //        material.IO_POSID = row["IO_POSID"].ToString();
        //        material.ITEM_ID = row["ITEM_ID"].ToString();
        //        material.ITEM_NAME = row["ITEM_NAME"].ToString();
        //        material.CART_ID = row["CART_ID"].ToString();
        //        Inputed.Add(material);
        //    }
            
        //    foreach (InputedMaterial input in Inputed)
        //        input.IsInBom = BOM.Find(x => x.ITEM_ID == input.ITEM_ID) != null ? true : false;
        //}

   }
}
