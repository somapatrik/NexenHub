using Microsoft.AspNetCore.Mvc;
using NexenHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace NexenHub.Pages.Shared.Components.MachineReportProdSum
{
    public class MachineReportProdSumViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(MachineProdReport reportData)
        {
            MachineProdReport ReportProd = reportData;
            return View("Default", ReportProd);
        }
    }
}
