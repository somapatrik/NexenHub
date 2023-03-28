using Microsoft.AspNetCore.Mvc;
using NexenHub.ViewModels;

namespace NexenHub.Pages.Shared.Components.InputPositions
{
    public class InputPositionsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string EQ_ID)
        {
            return View("Default", new InputPositionsViewModel(EQ_ID));
        }
    }
}
