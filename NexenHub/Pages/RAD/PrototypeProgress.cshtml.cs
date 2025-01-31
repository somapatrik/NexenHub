using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NexenHub.Class;
using NexenHub.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using static NexenHub.Pages.RAD.PrototypeProgressModel;

namespace NexenHub.Pages.RAD
{
    public class PrototypeProgressModel : PageModel
    {
        GlobalDatabase dbglob = new GlobalDatabase();

        #region Chart data
        public string ChartTitle { get; set; }
        public string xValues => JsonConvert.SerializeObject(_xLegend, Formatting.None);
        public string yReqValues => JsonConvert.SerializeObject(_yREQ, Formatting.None);
        public string yTBMValues => JsonConvert.SerializeObject(_yTBM, Formatting.None);
        public string yCUREValues => JsonConvert.SerializeObject(_yCUR, Formatting.None);
        public string yItemIdValues => JsonConvert.SerializeObject(_ItemId, Formatting.None);

        private List<string> _xLegend = new List<string>();
        private List<string> _ReqDates = new List<string>();
        private List<string> _EMR = new List<string>();
        private List<string> _yREQ = new List<string>();
        private List<string> _yTBM = new List<string>();
        private List<string> _yCUR = new List<string>();
        private List<string> _ItemId = new List<string>();
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
        public string selectedEMR { get; set; }

        [BindProperty]
        public string selectedItemID { get; set; }

        [BindProperty]
        public string selectedItemName { get; set; }

        public void OnGet()
        {
            CheckDates();
        }

        public void OnPostGenerate()
        {
            CheckDates();
            SetTitle();
            GenerateChart();
        }

        public List<SelectListItem> LoadTestTypes()
        {
            List<SelectListItem> Result = new List<SelectListItem>();

            DBOra db = new DBOra("select distinct(TEST_TYPE) from TB_PL_M_TEST_REQ where TEST_TYPE is not null order by TEST_TYPE");
            foreach (DataRow r in db.ExecTable().Rows)
            {
                Result.Add(new SelectListItem() { Value = r[0].ToString(), Text = r[0].ToString() });
            }

            return Result;
        }

        public void SetTitle()
        {
            if (!string.IsNullOrEmpty(selectedEMR))
                ChartTitle = selectedEMR.ToUpper();
            else
                ChartTitle = DateFrom.ToString("dd.MM.") + " - " + DateTo.ToString("dd.MM.");
        }

        private void CheckDates()
        {
            DateTime Now = DateTime.Now;

            // No dates
            if (DateFrom == DateTime.MinValue && DateTo == DateTime.MinValue)
            {
                DateFrom = Now.AddDays(-5);
                DateTo = Now;//Now.AddDays(5);
            }

            // Only From
            if (DateFrom != DateTime.MinValue && DateTo == DateTime.MinValue)
            {
                DateTime DateTo = Now;//DateFrom.AddDays(5) > Now ? Now : DateFrom.AddDays(5);
            }

            // Both but opposite
            if (DateTo < DateFrom)
            {
                DateTime temp = DateTo;
                DateTo = DateFrom;
                DateFrom = temp;
            }

            // Limit search
            if (DateTo > DateFrom.AddMonths(2))
                DateTo = DateFrom.AddMonths(2);

        }

        private void GenerateChart()
        {
            DataTable dt = dbglob.GetPrototypeProgressChart(DateFrom, DateTo, selectedEMR, selectedItemID, selectedItemName, SelectedTestType);

            foreach (DataRow r in dt.Rows)
            {
                // Filter OE / RE here
                bool dbOE = r["OE"].ToString() == "OE";
                bool dbRE = r["OE"].ToString() == "RE";

                if ((IsOE && dbOE) || (IsRe && dbRE) || (!IsOE && !IsRe))
                {
                    _ReqDates.Add(DateTime.Parse(r["REQ_DATE"].ToString()).ToString("dd.MM.yyyy"));
                    _EMR.Add(r["EMR_ID"].ToString());
                    _yREQ.Add(r["REQ_QTY"].ToString());
                    _yTBM.Add(r["TBM"].ToString());
                    _yCUR.Add(r["CURE"].ToString());
                    _ItemId.Add($"{r["ITEM_ID"].ToString()} [ {r["OE"].ToString()} ]");
                }
            }

            // Show dates only on first different EMR
            List<string> NewReqs = new List<string>();

            foreach (string reqdate in _ReqDates)
            {
                if (!NewReqs.Contains(reqdate))
                {
                    //First position
                    int found = _ReqDates.Count(x => x == reqdate);
                    NewReqs.Add(reqdate);
                    for (int i = 0; i < found - 1; i++)
                        NewReqs.Add("");
                }
            }

            int j = 0;
            _EMR.ForEach(x =>
            {
                //_xLegend.Add(x + ";" + NewReqs[j]);
                _xLegend.Add($"{x};{NewReqs[j]};{_ItemId[j]}");
                j++;
            });

        }

        
    }
}
