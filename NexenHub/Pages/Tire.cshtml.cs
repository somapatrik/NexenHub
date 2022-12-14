using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NetBarcode;
using NexenHub.Models;

namespace NexenHub.Pages
{
    public class TireModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string argBarcode { get; set; }
        public Tire tire { get; set; }
        public Barcode tireBarcode { get; set; }
        public bool showBarcode { get; set; }

        public void OnGet()
        {
            tire = new Tire(argBarcode);

            if (!string.IsNullOrEmpty(tire.Barcode)) 
            { 
                tireBarcode = new Barcode(tire.Barcode, NetBarcode.Type.Code128B);
                showBarcode = true;
            }
        }
    }
}
