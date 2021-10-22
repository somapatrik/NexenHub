using Microsoft.AspNetCore.Mvc;
using NexenHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Pages.Shared.Components.MachineProdChart
{
    public class MachineProdChartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string EQ_ID)
        {
            MachineProduction machProd = new MachineProduction(EQ_ID);
            machProd.LoadActProd();
            return View("Default", machProd);
        }

    }
}
