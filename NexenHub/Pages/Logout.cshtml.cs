using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexenHub.Class;
using System.Threading.Tasks;

namespace NexenHub.Pages
{
    public class LogoutModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string returnPath { get; set; }

        public async Task<IActionResult> OnGet()
        {

            await Login.Logout(HttpContext);

            if (string.IsNullOrEmpty(returnPath))
                return LocalRedirect("/index");
            else
                return LocalRedirect(returnPath);

        }
    }
}
