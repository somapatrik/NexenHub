using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexenHub.Models;
using NexenHub.Class;

namespace NexenHub.Pages
{
    public class LotModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string argLOT { get; set; }


        public LotItem lotitem;
        public Esl eslLayout { get; set; }

        private GlobalDatabase dbglob = new GlobalDatabase();
        public void OnGet()
        {

            if (argLOT.Length == 5)
                argLOT = dbglob.Cart2Lot(argLOT);

            lotitem = new LotItem(argLOT);
            lotitem.LoadHistory();
            lotitem.RemoveUselessHistory();

            eslLayout = new Esl();
            eslLayout.LOT_ID = argLOT;
            eslLayout.LoadLayout();
        }
    }
}
