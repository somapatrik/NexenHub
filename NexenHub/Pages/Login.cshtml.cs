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

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostLogin()
        {           
            bool logged = await Login.LogIn(HttpContext, Username, Password);

            if (logged)
                return LocalRedirect("/index");
            else
                return Page();
        }

        //public async Task<IActionResult> OnPostLogout()
        //{
        //    await Login.Logout(HttpContext);

        //    //if (logged)
        //    //    return LocalRedirect("/index");
        //    //else
        //        return Page();
        //}
    }
}
