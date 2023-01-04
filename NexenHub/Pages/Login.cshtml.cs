using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexenHub.Class;

namespace NexenHub.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string ErrorMsg { get; set; }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostLogin()
        {           
            bool logged = await Login.LogIn(HttpContext, Username, Password);

            if (logged) 
            { 
                return LocalRedirect("/index");
            }
            else
            {
                ErrorMsg = "Unable to login";
                return Page();
            }
        }
    }
}
