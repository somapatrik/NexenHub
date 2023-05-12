using Microsoft.AspNetCore.Mvc.RazorPages;
using NexenHub.ViewModels;

namespace NexenHub.Pages
{
    public class IndexModel : PageModel
    {

        public YearProd yearProd;

        public void OnGet()
        {
            yearProd = new YearProd();
        }

        
    }
}
