using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexenHub.Class;

namespace NexenHub.Pages
{
    public class MachineReportModel : PageModel
    {

        [BindProperty(SupportsGet =true)]
        public string eqArg { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime startArg { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime endArg { get; set; }

        [BindProperty]
        public string SelectedMachine { get; set; }

        [BindProperty]
        public string DateFrom { get; set; }

        [BindProperty]
        public string DateTo { get; set; }

        public List<MachineListObject> Machines;

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
                DateTo = startArg.ToString("yyyy-MM-dd");
            else
                DateTo = DateTime.Now.ToString("yyyy-MM-dd");

           // Generate();
        }

        public void OnPostGenerate()
        {
            CreateMachineList();
            Generate();
        }

        private void Generate()
        {
            if (ValidateInput())
            {

            }
        }

        private Boolean ValidateInput()
        {
            DateTime genFrom;
            DateTime genTo;

            if (!string.IsNullOrEmpty(SelectedMachine) && 
                !string.IsNullOrEmpty(DateFrom) && DateTime.TryParse(DateFrom, out genFrom) &&
                !string.IsNullOrEmpty(DateTo) && DateTime.TryParse(DateTo,out genTo))
            {
                return true;
            }

            return false;
        }

        private void CreateMachineList()
        {

            string[] DoNotShow =
            {
                "10004",
                "10005",
                "10006",
                "10007",
                "10017",
                "10024",
                "10032"
            };

            DataTable dt = dbglobal.GetMachineList(); ;
            Machines = new List<MachineListObject>();
            foreach (DataRow r in dt.Rows)
            {
                if (!DoNotShow.Contains(r["EQ_ID"].ToString()))
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
