using Microsoft.AspNetCore.Mvc;
using NexenHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NexenHub.Models;

namespace NexenHub.Pages.Shared.Components.MachineWoList
{
    public class MemberSearchViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("Default", new MemberSearchList());
        }
    }
}
