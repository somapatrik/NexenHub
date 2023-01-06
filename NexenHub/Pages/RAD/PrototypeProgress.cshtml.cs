using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NexenHub.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace NexenHub.Pages.RAD
{
    public class PrototypeProgressModel : PageModel
    {
        GlobalDatabase dbglob = new GlobalDatabase();

        #region Chart data
        public string ChartTitle { get; set; }
        public string xValues => JsonConvert.SerializeObject(_xLegend.Select(x => x), Formatting.None);
        public string yReqValues => JsonConvert.SerializeObject(_yREQ, Formatting.None);
        public string yTBMValues => JsonConvert.SerializeObject(_yTBM, Formatting.None);
        public string yCUREValues => JsonConvert.SerializeObject(_yCUR, Formatting.None);

        private List<string> _xLegend = new List<string>();
        private List<string> _ReqDates = new List<string>();
        private List<string> _EMR = new List<string>();
        private List<string> _yREQ = new List<string>();
        private List<string> _yTBM = new List<string>();
        private List<string> _yCUR = new List<string>();
        #endregion

        [BindProperty]
        public DateTime DateFrom { get; set; }

        [BindProperty]
        public DateTime DateTo { get; set; }

        [BindProperty]
        public string SelectedTestType { get; set; }

        [BindProperty]
        public List<SelectListItem> TestTypes => LoadTestTypes();

        [BindProperty]
        public bool IsOE { get; set; }

        [BindProperty]
        public bool IsRe { get; set; }

        [BindProperty]
        public bool selectedEMR { get; set; }

        [BindProperty]
        public bool selectedItemID { get; set; }

        [BindProperty]
        public bool selectedItemName { get; set; }



        public void OnGet()
        {
            CheckDates();
        }

        public List<SelectListItem> LoadTestTypes()
        {
            List<SelectListItem> Result = new List<SelectListItem>();

            DBOra db = new DBOra("select distinct(TEST_TYPE) from TB_PL_M_TEST_REQ where TEST_TYPE is not null");
            foreach (DataRow r in db.ExecTable().Rows)
            {
                Result.Add( new SelectListItem() {Value = r[0].ToString(), Text = r[0].ToString() });
            }

            return Result;
        }

        public void SetTitle()
        {
            ChartTitle = DateFrom.ToString("dd.MM.") + " - " + DateTo.ToString("dd.MM.");
        }

        public void OnPostGenerate()
        {
            CheckDates();
            SetTitle();
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

            // Limit searcch
            if (DateTo > DateFrom.AddMonths(1))
               DateFrom = DateTo.AddMonths(-1);

        }

        private void GenerateChart()
        {
            DataTable dt = dbglob.GetPrototypeProgressChart(DateFrom, DateTo);
            foreach (DataRow r in dt.Rows)
            {
                _ReqDates.Add(DateTime.Parse(r["REQ_DATE"].ToString()).ToString("dd.MM.yyyy"));
                _EMR.Add(r["EMR_ID"].ToString());
                _yREQ.Add(r["REQ_QTY"].ToString());
                _yTBM.Add(r["TBM"].ToString());
                _yCUR.Add(r["CURE"].ToString());
            }

            List<string> NewReqs = new List<string>();

            foreach(string reqdate in _ReqDates)
            {
                if (!NewReqs.Contains(reqdate)) 
                {
                    //First position
                    List<string> found = _ReqDates.FindAll(x => x == reqdate).ToList();
                    NewReqs.Add(reqdate);
                    for (int i = 1; i <= found.Count - 1; i++)
                        NewReqs.Add("");
                }
            }

            int j = 0;
            _EMR.ForEach(x =>
            {
                _xLegend.Add(x + ";" + NewReqs[j]);
                j++;
            });
            
        }
    }
}
