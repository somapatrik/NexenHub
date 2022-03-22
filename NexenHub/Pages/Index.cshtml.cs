using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NexenHub.Class;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using NexenHub.Models;
using System.Text;
using Newtonsoft.Json; 

namespace NexenHub.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        public string DaysInMonth { get; set; }
        public string TireDays { get; set; }
        public string GTDays { get; set; }
        public string GoalDay { get; set; }

        public int SelectedMonth;

        public string MinGt = " ";
        public string MaxGt = " ";
        public string AvgGt = " ";

        public string JandiTitle;
        public string JandiMsg;

        private int ProdGoal = 15000;

        private GlobalDatabase GlobalDb = new GlobalDatabase();

        public void OnGet()
        {
            FillChart();
            GetJandiMsg();
        }

        public void OnPostFilter(string id)
        {
            int MonthNum;
            if (int.TryParse(id, out MonthNum) && MonthNum >= 1 && MonthNum <= 12)
                FillChart(MonthNum);
            else
                FillChart();

            GetJandiMsg();
        }

        private void GetJandiMsg()
        {
            DataTable dt = GlobalDb.GetJandiMsg();
            if (dt.Rows.Count > 0) { 
                JandiTitle = dt.Rows[0]["TITLE"].ToString();
                JandiMsg = dt.Rows[0]["MSG"].ToString().Replace("\n", "<br>");
            }
        }

        private void FillChart(int FilterMonth = 0)
        {
            List<DateTime> ProdDaysInMonth = new List<DateTime>();
            List<ChartXYDate> GTprod = new List<ChartXYDate>();
            List<ChartXYDate> Tireprod = new List<ChartXYDate>();

            // Get number of days in a month
            int NumberOfDays;
            if (FilterMonth == 0)
            {
                SelectedMonth = DateTime.Now.Month;
                NumberOfDays = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            }
            else
            {
                SelectedMonth = FilterMonth;
                NumberOfDays = DateTime.DaysInMonth(DateTime.Now.Year, FilterMonth);
            }

            int Year = DateTime.Now.Year;
            int Month = DateTime.Now.Month;

            // Default values = 0, to fill days that are missing in database (in case some days do not produce tires)
            for (int Day = 1; Day <= NumberOfDays; Day++)
            {
                DateTime DayDate;

                if (FilterMonth == 0)
                    DayDate = new DateTime(Year, Month, Day);
                else
                    DayDate = new DateTime(Year, FilterMonth, Day);

                ProdDaysInMonth.Add(DayDate);
                GTprod.Add(new ChartXYDate() { Date = DayDate, Value = "0" }); 
                Tireprod.Add(new ChartXYDate() { Date = DayDate, Value = "0" });
            }

            // Rewrtite list with default values with dates we have
            DataTable dt = GlobalDb.GetProductionMonthDays(FilterMonth);
            foreach (DataRow row in dt.Rows)
            {
                DateTime prodtime = Convert.ToDateTime(row["DATETIME"].ToString());
                string qty = row["QTY"].ToString();
                string wc = row["WC_ID"].ToString();

                if (wc == "T")
                    GTprod.Find(g => g.Date == prodtime).Value = qty;
                else if (wc == "U")
                    Tireprod.Find(o => o.Date == prodtime).Value = qty;

            }

            // Format to JSON
           // DaysInMonth = JsonConvert.SerializeObject(ProdDaysInMonth, Formatting.Indented);
            List<string> FormatedDays = new List<string>();

            foreach (var DayMonth in ProdDaysInMonth)
                FormatedDays.Add(DayMonth.ToString("dd"));

            DaysInMonth = JsonConvert.SerializeObject(FormatedDays, Formatting.Indented);

            List<string> gtlist = new List<string>();
            List<string> tirelist = new List<string>();

            foreach (ChartXYDate gt in GTprod)
                gtlist.Add(gt.Value);

            foreach (ChartXYDate tire in Tireprod)
                tirelist.Add(tire.Value);

            TireDays = JsonConvert.SerializeObject(tirelist, Formatting.Indented);
            GTDays = JsonConvert.SerializeObject(gtlist, Formatting.Indented);

            List<string> goal = new List<string>();
            foreach (var day in ProdDaysInMonth)
            {
                if (day.Year <= 2021)
                {
                    if (day.Month < 9)
                        goal.Add("14000");
                    else if (day.Month == 9 || day.Month == 10)
                        goal.Add("15000");
                    else
                        goal.Add(ProdGoal.ToString());
                }
                else
                {
                    goal.Add(ProdGoal.ToString());
                }
                
            }

            GoalDay = JsonConvert.SerializeObject(goal, Formatting.Indented);

            // Set min,max,avg
            DateTime yesterday = DateTime.Now.AddDays(-1);
            DateTime compareTo = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 23, 59, 59);
            //DateTime compareTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 1, 23, 59, 59);
            
            List<ChartXYDate> ProdValues = GTprod.FindAll(gt => gt.Date <= compareTo); // only past
            List<ChartXYDate> ProdMinMax = ProdValues.FindAll(gt => gt.Value !="0");

            if (ProdMinMax.Count > 0)
            {
                MinGt = ProdMinMax.Min(gt => int.Parse(gt.Value)).ToString("### ###");
                MaxGt = ProdMinMax.Max(gt => int.Parse(gt.Value)).ToString("### ###");
                AvgGt = ProdMinMax.Average(gt => int.Parse(gt.Value)).ToString("### ###");
            }
        }
    }
}
