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
            else if (WC_ID == "B")
                WC_NAME = "Bead";
            else if (WC_ID == "C")
                WC_NAME = "Calendering";
            else if (WC_ID == "T")
                WC_NAME = "TBM";
            else if (WC_ID == "U")
                WC_NAME = "Curing";
        }

        private void LoadMachines()
        {
            Machines = new List<MachineBasicInfo>();
            foreach (DataRow row in dbglob.GetMachineList(WC_ID: WC_ID).Rows)
                if (!GlobalSettings.OEEIgnoredMachines.Contains(row["EQ_ID"].ToString()) && row["FACT_ID"].ToString()!="NEX2" )
                    Machines.Add(new MachineBasicInfo(row["EQ_ID"].ToString()));
        }
    }
}
