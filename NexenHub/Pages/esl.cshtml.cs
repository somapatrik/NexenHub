using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexenHub.Class;
using NexenHub.Models;

namespace NexenHub.Pages
{
    public class eslModel : PageModel
    {
        public string GeneratedLayout { get; set; }
        public string GeneratedLayout2 { get; set; }

        public Esl EslModel;
        public GlobalDatabase globalDatabase = new GlobalDatabase();

        public async Task<IActionResult> OnGet(string cartid)
        {
            if (cartid.Length == 5)
            {
                //cartid = cartid.ToUpper();
                EslModel = new Esl(cartid);
                if (EslModel.VALID)
                {
                    EslModel.LoadLayout();
                    await Task.Run(() => 
                    { 
                        GeneratedLayout = EslModel.GetLayout();
                        GeneratedLayout2 = EslModel.GetLayoutBack();
                    });
                }
                    
            }

            return Page();
        }


        public void SetNotFoundHtml()
        {

        }

        public void SetWrongInputHtml(string input)
        {

        }

    }
}
