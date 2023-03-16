using Microsoft.IdentityModel.Tokens;
using NexenHub.Class;
using NexenHub.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace NexenHub.ViewModels
{
    public class WorkSectionViewModel
    {
        public string WC_ID { get; set; }
        public string WC_NAME { get; set; }
        public string FACT_ID { get; set; }
        public WorkSectionOee WC_OEE { get; set; }

       // private GlobalDatabase dbglob = new GlobalDatabase();

        public WorkSectionViewModel(string wc_id, string factory_id = "")
        {
            WC_ID = wc_id.ToUpper();
            FACT_ID = factory_id.ToUpper();

            SetName();

            WC_OEE = new WorkSectionOee(WC_ID,FACT_ID);
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

    }
}
