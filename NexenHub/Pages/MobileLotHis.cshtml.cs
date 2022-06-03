using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexenHub.Models;

namespace NexenHub.Pages
{
    public class MobileLotHis : PageModel
    {
        public LotItem lot { get; set; }

        public void OnGet(string argLot)
        {
            lot = new LotItem(argLot);
            lot.LoadHistory();
            lot.RemoveUselessHistory();

        }
    }
}
