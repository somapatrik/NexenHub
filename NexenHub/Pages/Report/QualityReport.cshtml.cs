using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NexenHub.Class;
using NexenHub.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.PortableExecutable;

namespace NexenHub.Pages.Report
{
    public class QualityReportModel : PageModel
    {

        [BindProperty]
        public string selectedMachine { get; set; }
        public SelectList machineFilter { get; set; }

        [BindProperty]
        public int selectedYear { get; set; }
        public SelectList yearFilter { get; set; }

        [BindProperty]
        public int selectedMonth { get; set; }
        public SelectList monthFilter { get; set; }

        public bool GeneratorEnabled { get; set; }
        public MachineQuality QualityModel { get; set; }


        private List<MachineListObject> Machines { get; set; }
        private List<int> enabledMonths { get; set; }
        private List<int> enabledYears { get; set; }


        private GlobalDatabase database = new GlobalDatabase();

        public void OnGet()
        {
            LoadMachines();
            LoadYears();
            LoadMonths();

            SelectLastMonth();
        }

        public void OnPostGenerateReport()
        {
            LoadMachines();
            LoadYears();
            LoadMonths();

            if (ValidSelection())
                Generate();
        }

        public void Generate()
        {
            DateTime firstDay = new DateTime(selectedYear, selectedMonth, 1, 0, 0, 0);
            DateTime lastDay = firstDay.AddMonths(1).AddDays(-1);

            QualityModel = new MachineQuality(selectedMachine, firstDay,lastDay);
        }

        private bool ValidSelection()
        {
            GeneratorEnabled = true;

            DateTime now = DateTime.Now;
            if (now.Year == selectedYear && selectedMonth >= now.Month)
                GeneratorEnabled = false;
            

            
            return GeneratorEnabled;
        }

        private void SelectLastMonth()
        {
            DateTime LastMonth = DateTime.Now.AddMonths(-1);
            int selectYear = LastMonth.Year;
            int selectMonth = LastMonth.Month;

            if (enabledMonths.Contains(selectMonth))
                selectedMonth = selectMonth;

            if (enabledYears.Contains(selectYear))
                selectedYear = selectYear;

        }

        private void LoadMonths()
        {
            enabledMonths = new List<int>();
            for (int i = 1; i <= 12; i++)
                enabledMonths.Add(i);

            monthFilter = new SelectList(enabledMonths);
        }

        private void LoadYears()
        {
            enabledYears = new List<int>();
            for (int i = 2023; i >= 2021; i--)
                enabledYears.Add(i);


            yearFilter = new SelectList(enabledYears);
        }

        private void LoadMachines()
        {
            DataTable dt = database.GetMachineList(); ;
            Machines = new List<MachineListObject>();

            foreach (DataRow r in dt.Rows)
            {
                if (!GlobalSettings.IgnoredMachines.Contains(r["EQ_ID"].ToString()) &&
                    r["WC_ID"].ToString() != "U" &&
                    r["WC_ID"].ToString() != "M" &&
                    r["WC_ID"].ToString() != "T"
                    )
                {
                    Machines.Add(new MachineListObject
                    {
                        EQ_ID = r["EQ_ID"].ToString(),
                        Name = r["Name"].ToString(),
                        WC_ID = r["WC_ID"].ToString(),
                        PROC_ID = r["PROC_ID"].ToString()
                    });
                }
            }

            Machines = Machines.OrderBy(m => m.WC_ID).ToList();

            machineFilter = new SelectList(Machines, nameof(MachineListObject.EQ_ID), nameof(MachineListObject.Name));
        }
    }
}
