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
   public class MachineProfileModel : PageModel
    {
        public string IArg { get; set; }
        public string chartlabels { get; set; }
        public string chartdataset { get; set; }

        public WorkOrder WO { get; set; }

        private GlobalDatabase dbglob = new GlobalDatabase();

        private JObject jdata = new JObject();

        private ChartDownTimeDataSet ChartDataScript;

        private List<string> labels;


        public void OnGet(string EQ_ID)
        {
            if (EQ_ID != null)
            {
                IArg = EQ_ID;

                // Get downtimes
                DataTable dt = dbglob.GetNonWorkSum(EQ_ID);
                ChartDataScript = new ChartDownTimeDataSet();

                FillDataScript(dt);

                chartdataset = JsonConvert.SerializeObject(ChartDataScript, Formatting.Indented);
                chartlabels = JsonConvert.SerializeObject(labels, Formatting.Indented);

                // Get WO
                WO = new WorkOrder();
                WO.LoadFromMachine(EQ_ID);

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
