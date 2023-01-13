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
        public string selectedEMR { get; set; }

        [BindProperty]
        public string selectedItemID { get; set; }

        [BindProperty]
        public string selectedItemName { get; set; }

        public string formatLocations => JsonConvert.SerializeObject(Locations);
        public string formatEMRdata => JsonConvert.SerializeObject(EMRS);
        public string formatDefectCharts => JsonConvert.SerializeObject(defectCharts);
        public List<LocationObject> Locations { get; set; }
        public List<EMR> EMRS { get; set; }
        public List<DefectChart> defectCharts { get; set; }

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
                }
            }

            // Show dates only on first different EMR
            List<string> NewReqs = new List<string>();

            foreach (string reqdate in _ReqDates)
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

            Locations = new List<LocationObject>();
            EMRS = new List<EMR>();
            defectCharts = new List<DefectChart>();

            int j = 0;
            _EMR.ForEach(x =>
            {
                // Generate data for main chart
                _xLegend.Add(x + ";" + NewReqs[j]);
                j++;

                // Generate location data
                LocationObject locs = new LocationObject();
                locs.EMR = x;
                foreach (DataRow r in dbglob.GetEMRLocations(x).Rows)
                {
                    locs.Values.Add(r["CNT_LOC"].ToString());
                    locs.Labels.Add(r["WH_ID"].ToString());
                }

                Locations.Add(locs);

                // Generate EMR basic info
                EMRS.Add(new EMR(x));

                // Generate defect locations
                DefectChart defectChart = new DefectChart(x);
                DataTable dt = dbglob.GetEMRDefects(x);
                foreach (DataRow r in dt.Rows)
                {
                    defectChart.AddLabel(r["BAD_ID"].ToString());
                    //defectChart.AddDataSet(r["PROTOTYPE_BOM_VER"].ToString());
                    //defectChart.AddValue(r["PROTOTYPE_BOM_VER"].ToString(), r["BAD_ID"].ToString(), r["CNT"].ToString());
                }

                foreach (DataRow r in dt.Rows)
                {
                    //defectChart.AddLabel(r["BAD_ID"].ToString());
                    defectChart.AddDataSet(r["PROTOTYPE_BOM_VER"].ToString());
                    defectChart.AddValue(r["PROTOTYPE_BOM_VER"].ToString(), r["BAD_ID"].ToString(), r["CNT"].ToString());
                }

                defectCharts.Add(defectChart);
            });

        }

        public class DefectChart
        {
            public string EMR { get; set; }
            public List<string> Labels { get; set; }
            public List<ChartDataSet> DataSets { get; set; }

            public DefectChart(string emr)
            {
                EMR = emr;
                DataSets = new List<ChartDataSet>();
                Labels = new List<string>();
            }

            public void AddLabel(string label)
            {
                if (!Labels.Contains(label))
                    Labels.Add(label);
            }

            public void AddDataSet(string dsName)
            {
                if (DataSets.Find(x => x.Label == dsName) == null) 
                {
                    ChartDataSet dSet = new ChartDataSet() { Label = dsName };
                    Labels.ForEach(x => dSet.Data.Add("NaN"));
                    DataSets.Add(dSet);
                }
            }

            public void AddValue(string dSet,string label, string value)
            {
                int i = Labels.FindIndex(l => l == label);
                try
                {
                    DataSets.Find(d => d.Label == dSet).Data[i] = value;
                }
                catch { }
            }

        }

        public class ChartDataSet
        {
            public string Label { get; set; }
            public List<string> Data { get; set; }

            public ChartDataSet()
            {
                Data = new List<string>();
            }

        }

        public class LocationObject
        {
            public string EMR { get; set; }
            public List<string> Labels { get; set; }
            public List<string> Values { get; set; }

            public LocationObject()
            {
                EMR = "";
                Labels = new List<string>();
                Values = new List<string>();
            }

        }

    }
}
