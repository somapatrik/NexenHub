using Microsoft.AspNetCore.Mvc;
using NexenHub.ViewModels;

namespace NexenHub.Pages.Shared.Components.MachineMaterialInUse
{
    public class MachineOeePieViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string EQ_ID)
        {
            return View("Default", new MachineOeePieViewModel(EQ_ID));
        }
    }
}
