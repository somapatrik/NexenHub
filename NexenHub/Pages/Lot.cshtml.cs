using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexenHub.Models;

namespace NexenHub.Pages
{
    public class LotModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string argLOT { get; set; }

        public LotItem lotitem;

        public void OnGet()
        {
            lotitem = new LotItem(argLOT);
            lotitem.LoadHistory();
        }
    }
}
