using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using NexenHub.Models;
using System;

namespace NexenHub.Pages
{
    public class WorkorderModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string argWo { get; set; }

        public WorkOrder WO { get; set; }

        public void OnGet()
        {
            WO = new WorkOrder();
            WO.LoadById(argWo);                
        }

        public string DateToString(DateTime myTime)
        {
            return myTime == DateTime.MinValue ? "" : myTime.ToString();
        }
    }
}
