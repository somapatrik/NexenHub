using Microsoft.AspNetCore.Mvc;
using NexenHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Pages.Shared.Components.LotItemHis
{
    public class LotItemHisViewComponent :ViewComponent
    {
        public IViewComponentResult Invoke(LotItem info)
        {
            return View("Default", info);
        }
    }
}
