using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexenHub.Class;

namespace NexenHub.Pages.Cockpit
{
    public class MachineModel : PageModel
    {
        public string EQ_ID { get; set; }

        public MachineBasicInfo machineInfo { get; set; }

        public void OnGet(string EQ_ID)
        {
            this.EQ_ID = EQ_ID;
            machineInfo = new MachineBasicInfo(EQ_ID);
        }
    }
}
