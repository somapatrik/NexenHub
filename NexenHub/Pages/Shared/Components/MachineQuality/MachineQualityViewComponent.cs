using Microsoft.AspNetCore.Mvc;
using NexenHub.Models;

namespace NexenHub.Pages.Shared.Components.MachineQuality
{
    public class MachineQualityViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(Models.MachineQuality model)
        {
            return View("Default", model);
        }
    }
}
