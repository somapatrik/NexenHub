using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using NexenHub.Class;
using NexenHub.Models;
using System;

namespace NexenHub.Pages
{
    public class WorkorderModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string argWo { get; set; }

        public WorkOrder WO { get; set; }

        public Item itemWo { get; set; }

        public MachineBasicInfo machine { get; set; }

        public void OnGet()
        {
            WO = new WorkOrder();
            WO.LoadById(argWo);

            if (!string.IsNullOrEmpty(WO.ITEM_ID))
                itemWo = new Item(WO.ITEM_ID);

            machine = new MachineBasicInfo(WO.EQ_ID);
        }

        public string DateToString(DateTime myTime)
        {
            return myTime == DateTime.MinValue ? "" : myTime.ToString();
        }
    }
}
