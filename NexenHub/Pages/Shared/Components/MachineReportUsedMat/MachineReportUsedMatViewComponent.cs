using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NexenHub.Models;

namespace NexenHub.Pages.Shared.Components.MachineReportUsedMat
{
        public class MachineReportUsedMatViewComponent : ViewComponent
        {
            public IViewComponentResult Invoke(MachineUsedMat usedMat)
            {
                //MachineProdReport ReportProd = reportData;
                return View("Default", usedMat);
            }
        }
}
