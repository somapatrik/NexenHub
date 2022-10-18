using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexenHub.Models;
using NexenHub.Class;
using NetBarcode;

namespace NexenHub.Pages
{
    public class LotModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string arglot { get; set; }
        
        public LotItem lotitem { get; set; }

        public Barcode lotBarcode { get; set; }

        private GlobalDatabase dbglob = new GlobalDatabase();


        public void OnGet()
        {

            if (arglot.Length == 5)
                arglot = dbglob.Cart2Lot(arglot);


            lotitem = new LotItem(arglot);
            lotitem.LoadHistory();
            lotitem.RemoveUselessHistory();

            lotBarcode = new Barcode(lotitem.LOT_ID, NetBarcode.Type.Code128B) ;

        }
    }
}
