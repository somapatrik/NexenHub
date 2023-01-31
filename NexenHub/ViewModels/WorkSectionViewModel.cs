using Microsoft.IdentityModel.Tokens;
using NexenHub.Class;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace NexenHub.ViewModels
{
    public class WorkSectionViewModel
    {
        public string WC_ID { get; set; }
        public string WC_NAME { get; set; }

        public List<MachineBasicInfo> Machines { get; set; }

        private GlobalDatabase dbglob = new GlobalDatabase();

        public WorkSectionViewModel(string wc_id)
        {
            WC_ID = wc_id.ToUpper();
            SetName();
            LoadMachines();
        }

        private void SetName()
        {
            if (WC_ID == "M")
                WC_NAME = "Mixing";
            else if (WC_ID == "E")
                WC_NAME = "Extrusion";
            else if (WC_ID == "P")
                WC_NAME = "Cutting";
            else if (WC_ID == "C")
                WC_NAME = "Calendering";
            else if (WC_ID == "T")
                WC_NAME = "TBM";
        }

        private void LoadMachines()
        {
            Machines = new List<MachineBasicInfo>();
            foreach (DataRow row in dbglob.GetMachineList(WC_ID: WC_ID).Rows)
                if (!GlobalSettings.IgnoredMachines.Contains(row["EQ_ID"].ToString()))
                    Machines.Add(new MachineBasicInfo(row["EQ_ID"].ToString()));
        }
    }
}
