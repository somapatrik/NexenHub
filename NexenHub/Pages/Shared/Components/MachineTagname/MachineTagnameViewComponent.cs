using Microsoft.AspNetCore.Mvc;
using NexenHub.Class;
using NexenHub.ViewModels;

namespace NexenHub.Pages.Shared.Components.InputPositions
{
    public class MachineTagnameViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(MachineBasicInfo info)
        {
            return View("Default", new MachineTagnameViewModel(info));
        }
    }
}
