﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NexenHub.Pages
{
    public class eslModel : PageModel
    {

        public string Generated { get; set; }

        public void OnGet()
        {
            Generated = @"<i>Nečum</i>";
        }
    }
}
