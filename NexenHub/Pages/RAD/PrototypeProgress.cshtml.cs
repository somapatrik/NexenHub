using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using NexenHub.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace NexenHub.Pages.RAD
{
    public class PrototypeProgressModel : PageModel
    {
        GlobalDatabase dbglob = new GlobalDatabase();

        public string xValues => JsonConvert.SerializeObject(_xLegend.Select(x => x.ToString("yyyy-MM-dd")), Formatting.None);

        public string xEMRValues => JsonConvert.SerializeObject(_EMR, Formatting.None);

        public string yReqValues => JsonConvert.SerializeObject(_yREQ, Formatting.None);
        public string yTBMValues => JsonConvert.SerializeObject(_yTBM, Formatting.None);
        public string yCUREValues => JsonConvert.SerializeObject(_yCUR, Formatting.None);

        private List<DateTime> _xLegend = new List<DateTime>();

        private List<string> _EMR = new List<string>();
        private List<string> _yREQ = new List<string>();
        private List<string> _yTBM = new List<string>();
        private List<string> _yCUR = new List<string>();

        [BindProperty(SupportsGet =true)]
        public DateTime DateFrom { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime DateTo { get; set; }

        public void OnGet()
        {
            CheckDates();
            GenerateChart();
        }

        public void OnPostGenerate()
        {
            CheckDates();
            GenerateChart();
        }

        private void CheckDates()
        {
            DateTime Now = DateTime.Now;

            // No dates
            if (DateFrom == DateTime.MinValue && DateTo == DateTime.MinValue) 
            { 
                DateFrom = Now.AddDays(-7);
                DateTo = Now;
            }

            // Only From
            if (DateFrom != DateTime.MinValue && DateTo == DateTime.MinValue)
            {
                DateTime DateTo = DateFrom.AddDays(7) > Now ? Now : DateFrom.AddDays(7);
            }

            // Both but opposite
            if (DateTo < DateFrom)
            {
                DateTime temp = DateTo;
                DateTo = DateFrom;
                DateFrom = temp;
            }

        }

        private void GenerateChart()
        {
            DataTable dt = dbglob.GetPrototypeProgressChart(DateFrom, DateTo);
            foreach (DataRow r in dt.Rows)
            {
                _xLegend.Add( DateTime.Parse(r["REQ_DATE"].ToString()));
                _EMR.Add(r["EMR_ID"].ToString());
                _yREQ.Add(r["REQ_QTY"].ToString());
                _yTBM.Add(r["TBM"].ToString());
                _yCUR.Add(r["CURE"].ToString());
            }
        }
    }
}
