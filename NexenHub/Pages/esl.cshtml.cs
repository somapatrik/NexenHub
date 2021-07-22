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
        public int LoadStatus { get; set; }
        public string Argument { get; set; }

        public Esl EslModel;
        public GlobalDatabase globalDatabase = new GlobalDatabase();

        public async Task<IActionResult> OnGet(string cartid)
        {
            Argument = cartid;
            if (cartid.Length == 5)
            {
                EslModel = new Esl();
                await Task.Run(() =>
                {
                    EslModel.CART_ID = cartid;
                });

                if (EslModel.VALID)
                {
                    EslModel.LoadLayout();
                    GeneratedLayout = EslModel.GetLayout();
                }
                else
                {
                    LoadStatus = 1;
                }   
            }
            else
            {
                LoadStatus = 2;
            }

            return Page();
        }
    }
}
