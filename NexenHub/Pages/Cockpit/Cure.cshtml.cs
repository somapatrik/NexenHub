using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexenHub.Class;
using System.Collections.Generic;
using System.Data;

namespace NexenHub.Pages.Cockpit
{
    public class CureModel : PageModel
    {
        public List<MachineBasicInfo> Phase1 { get; set; }
        public List<MachineBasicInfo> Phase2 { get; set; }

        private GlobalDatabase database = new GlobalDatabase();
        public void OnGet()
        {
            LoadEQ();
        }

        private void LoadEQ()
        {
            Phase1 = new List<MachineBasicInfo>();
            Phase2 = new List<MachineBasicInfo>();

            foreach (DataRow row in database.GetMachineList(WC_ID: "U", FACT_ID: "NEX1").Rows)
            {
                Phase1.Add(new MachineBasicInfo(row["EQ_ID"].ToString()));
            }

            foreach (DataRow row in database.GetMachineList(WC_ID: "U", FACT_ID: "NEX2").Rows)
            {
                Phase2.Add(new MachineBasicInfo(row["EQ_ID"].ToString()));
            }
        }
    }
}
