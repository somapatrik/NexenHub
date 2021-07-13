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

        public Esl EslModel;
        public GlobalDatabase globalDatabase = new GlobalDatabase();

        public async Task<IActionResult> OnGet(string cartid)
        {
            if (cartid.Length == 5)
            {
                EslModel = new Esl(cartid);
                if (EslModel.VALID)
                {
                    EslModel.LoadLayout();
                    await Task.Run(() => 
                    { 
                        GeneratedLayout = EslModel.GetLayout();
                    });
                }
                else
                {
                    GeneratedLayout = SetNotFoundHtml(cartid);
                }
                    
            }
            else
            {
                GeneratedLayout = SetWrongInputHtml(cartid);
            }

            return Page();
        }


        public string SetNotFoundHtml(string input)
        {
            return "<div class=\"p-3 mb-2 bg-danger text-white text-center\">Unable to find: " + input + "</div>";
        }

        public string SetWrongInputHtml(string input)
        {
            return "<div class=\"p-3 mb-2 bg-warning text-dark text-center\">Wrong input: " + input + "</div>";
    }

    }
}
