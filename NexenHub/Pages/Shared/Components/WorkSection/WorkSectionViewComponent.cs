using Microsoft.AspNetCore.Mvc;
using NexenHub.Models;
using NexenHub.ViewModels;

namespace NexenHub.Pages.Shared.Components.WorkSection
{
    public class WorkSectionViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string WC_ID)
        {
            return View("Default", new WorkSectionViewModel(WC_ID));
        }
    }
}
