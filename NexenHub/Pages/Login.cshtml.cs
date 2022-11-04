using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NexenHub.Pages
{
    public class LoginModel : PageModel
    {
        public void OnGet()
        {
            string hovno = HttpContext.Session.GetString("Test String");
            HttpContext.Session.SetString("Test String", "Hovno");
            hovno = HttpContext.Session.GetString("Test String");
        }
    }
}
