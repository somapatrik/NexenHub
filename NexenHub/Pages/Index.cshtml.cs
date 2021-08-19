﻿using Microsoft.AspNetCore.Mvc;
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

        private int ProdGoal;


        private GlobalDatabase GlobalDb = new GlobalDatabase();

        public void OnGet()
        {
            ProdGoal = 15000;
            FillChart();
        }

        private void FillChart()
        {
            List<DateTime> ProdDaysInMonth = new List<DateTime>();
            List<ChartXYDate> GTprod = new List<ChartXYDate>();
            List<ChartXYDate> Tireprod = new List<ChartXYDate>();

            // Get number of days in a month
            int NumberOfDays = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            int Year = DateTime.Now.Year;
            int Month = DateTime.Now.Month;

            // Default values = 0, to fill days that are missing in database (in case some days do not produce tires)
            for (int Day = 1; Day <= NumberOfDays; Day++)
            {
                DateTime DayDate = new DateTime(Year, Month, Day);

                ProdDaysInMonth.Add(DayDate);
                GTprod.Add(new ChartXYDate() { Date = DayDate, Value = "0" }); 
                Tireprod.Add(new ChartXYDate() { Date = DayDate, Value = "0" });
            }

            // Rewrtite list with default values with dates we have
            DataTable dt = GlobalDb.GetProductionMonthDays();
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
            DaysInMonth = JsonConvert.SerializeObject(ProdDaysInMonth, Formatting.Indented);

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
                goal.Add(ProdGoal.ToString());

            GoalDay = JsonConvert.SerializeObject(goal, Formatting.Indented);


        }
    }
}
