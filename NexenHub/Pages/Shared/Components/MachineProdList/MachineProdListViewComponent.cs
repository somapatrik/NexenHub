using Microsoft.AspNetCore.Mvc;
using NexenHub.ViewModels;

namespace NexenHub.Pages.Shared.Components.MachineMaterialInUse
{
    public class MachineProdListViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string EQ_ID)
        {
            return View("Default", new MachineProdListViewModel(EQ_ID));
        }
    }
}
