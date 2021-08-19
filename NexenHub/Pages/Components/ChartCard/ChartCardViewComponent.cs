using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NexenHub.Pages.Components.ChartCard
{
    [ViewComponent]
    public class ChartCardViewComponent : ViewComponent
    {


        public string ChartName { get; set; }

        public ChartCardViewComponent()
        {

        }

        public IViewComponentResult Invoke(string name)
        {
            ChartName = name;
            return View("Default", name);
        }

    }
}
