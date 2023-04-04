using Microsoft.AspNetCore.Mvc;
using NexenHub.ViewModels;

namespace NexenHub.Pages.Shared.Components.WoLine
{
    public class WoLineViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string eqid)
        {
            return View("Default", new WoLineViewModel(eqid));
        }
    }
}
