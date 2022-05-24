using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using NetBarcode;

namespace NexenHub.Pages
{
    public class barcodeModel : PageModel
    {
        public string CartId { get; set; }
        public string BARCODE_128 { get; set; }

        public void OnGet(string argCart)
        {
            CartId = argCart.ToUpper();
            var bar = new Barcode(CartId, NetBarcode.Type.Code128B);
            BARCODE_128 = bar.GetBase64Image();
        }
    }
}
