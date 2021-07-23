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

        public async Task<IActionResult> OnGet(string input)
        {
            // CART_ID = 5 chars
            // LOT_ID = 15 chars
            Argument = input;

            if (input.Length == 5 || input.Length == 15)
            {
                EslModel = new Esl();
                await Task.Run(() =>
                {
                    if (input.Length == 5)
                        EslModel.CART_ID = input;
                    else if (input.Length == 15)
                        EslModel.LOT_ID = input;
                });

                if (EslModel.VALID)
                {
                    // Loads layout extra so that when using /API/ESL/CARTID it doesnt have to load
                    await Task.Run(() => { EslModel.LoadLayout(); });
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
