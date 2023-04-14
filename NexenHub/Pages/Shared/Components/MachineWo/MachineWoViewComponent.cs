using Microsoft.AspNetCore.Mvc;
using NexenHub.ViewModels;

namespace NexenHub.Pages.Shared.Components.MachineWoList
{
    public class MachineWoViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string EQ_ID)
        {
            return View("Default", new MachineWoViewModel(EQ_ID));
        }
    }
}
