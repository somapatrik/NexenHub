using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexenHub.Class;
using NexenHub.Models;

namespace NexenHub.Pages.Report
{
    public class MachineReportModel : PageModel
    {

        #region URL arguments

        [BindProperty(SupportsGet =true)]
        public string eqArg { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime startArg { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime endArg { get; set; }

        #endregion

        #region GUI variables

        [BindProperty]
        public string SelectedMachine { get; set; }

        [BindProperty]
        public string DateFrom { get; set; }

        [BindProperty]
        public string DateTo { get; set; }

        public bool IsValid { get; set; }

        public List<MachineListObject> Machines;

        #endregion

        public MachineQuality qualityData;

        public MachineProdReport reportData;

        public MachineUsedMat reportUsedMaterial;

        private GlobalDatabase dbglobal = new GlobalDatabase();

        public void OnGet()
        {
            CreateMachineList();
            if (eqArg != null && Machines.Find(m => m.EQ_ID == eqArg) != null)
                SelectedMachine = eqArg;

            if (startArg != null && startArg != DateTime.MinValue)
                DateFrom = startArg.ToString("yyyy-MM-dd");
            else
                DateFrom = DateTime.Now.ToString("yyyy-MM-dd");

            if (endArg != null && endArg != DateTime.MinValue)
                DateTo = endArg.ToString("yyyy-MM-dd");
            else
                DateTo = DateFrom;
            //DateTo = DateTime.Now.ToString("yyyy-MM-dd");

            if (!string.IsNullOrEmpty(SelectedMachine))
                Generate();

        }

        public void OnPostGenerate()
        {
            CreateMachineList();
            Generate();
        }

        private void Generate()
        {
            IsValid = ValidateInput(); 
        }

        private Boolean ValidateInput()
        {
            DateTime genFrom;
            DateTime genTo;

            if (!string.IsNullOrEmpty(SelectedMachine) && 
                !string.IsNullOrEmpty(DateFrom) && DateTime.TryParse(DateFrom, out genFrom) &&
                !string.IsNullOrEmpty(DateTo) && DateTime.TryParse(DateTo,out genTo))
            {
                qualityData = new MachineQuality(SelectedMachine, genFrom, genTo);
                reportData = new MachineProdReport(SelectedMachine, genFrom, genTo);
                reportUsedMaterial = new MachineUsedMat(SelectedMachine, genFrom, genTo);
                //reportDowntTimes = new MachineDownTime(SelectedMachine, genFrom, genTo);
                return true;
            }

            return false;
        }

        private void CreateMachineList()
        {
            DataTable dt = dbglobal.GetMachineList(); ;
            Machines = new List<MachineListObject>();
            foreach (DataRow r in dt.Rows)
            {
                if (!GlobalSettings.IgnoredMachines.Contains(r["EQ_ID"].ToString()) && r["WC_ID"].ToString() != "U")
                    Machines.Add(new MachineListObject
                    {
                        EQ_ID = r["EQ_ID"].ToString(),
                        Name = r["Name"].ToString(),
                        WC_ID = r["WC_ID"].ToString(),
                        PROC_ID = r["PROC_ID"].ToString()
                    });
            }
        }
    }
}
