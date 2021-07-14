using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NexenHub.Pages
{
    public class CartsInfoModel : PageModel
    {

        public bool ShowFrame { set; get; }

        [BindProperty]
        public string CartID { set; get; }

        public void OnGet()
        {

        }

        public void OnPostShow()
        {
            if (!string.IsNullOrEmpty(CartID))
            {
                ShowFrame = true;

            }
            else
            {
                ShowFrame = false;
            }
        }
    }
}
