using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NexenHub.Pages
{
    public class MachineProfileModel : PageModel
    {
        public string IArg { get; set; }

        public void OnGet(string EQ_ID)
        {
            if (EQ_ID != null)
            {
                IArg = EQ_ID;

            }
        }

    }
}
