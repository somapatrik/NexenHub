using Microsoft.AspNetCore.Mvc;
using NexenHub.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Pages.Shared.Components.LotParents
{
    public class LotParentsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string lot)
        {
            return View("Default", new LotParentsViewModel(lot));
        }
    }
}
