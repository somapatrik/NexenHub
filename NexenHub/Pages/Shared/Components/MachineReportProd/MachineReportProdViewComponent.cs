using Microsoft.AspNetCore.Mvc;
using NexenHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Pages.Shared.Components.MachineReportProd
{
    public class MachineReportProdViewComponent : ViewComponent
    {        
        public IViewComponentResult Invoke(string EQ_ID)
        {
            MachineProdReport ReportProd = new MachineProdReport(EQ_ID);
            return View("Default", ReportProd);
        }
    }
}
