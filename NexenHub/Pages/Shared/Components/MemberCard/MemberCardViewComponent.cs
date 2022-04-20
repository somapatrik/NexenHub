using Microsoft.AspNetCore.Mvc;
using NexenHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Pages.Shared.Components.MachineWoList
{
    public class MemberCardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(Member member)
        {
            return View("Default", member);
        }
    }
}
