using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NexenHub.Models;

namespace NexenHub.Pages.Shared.Components.LotItemInfo
{
    public class LotItemInfoViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(LotItem info)
        {
            return View("Default", info);
        }

    }
}
