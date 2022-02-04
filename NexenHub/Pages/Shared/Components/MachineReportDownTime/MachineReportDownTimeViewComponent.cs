using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NexenHub.Models;

namespace NexenHub.Pages.Shared.Components.MachineReportDownTime
{
    public class MachineReportDownTimeViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(MachineDownTime downTime)
        {
            return View("Default", downTime);
        }
    }
}
