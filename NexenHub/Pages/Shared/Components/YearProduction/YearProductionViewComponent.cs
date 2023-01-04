using Microsoft.AspNetCore.Mvc;
using NexenHub.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Pages.Shared.Components.YearProduction
{
    public class YearProductionViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(YearProd yearProd)
        {
            return View("Default", yearProd);
        }
    }
}
