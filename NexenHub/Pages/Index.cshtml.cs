using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NexenHub.Class;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using NexenHub.Models;

namespace NexenHub.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public string Production { get; set; }
                
        public void OnGet()
        {
            Epaper epaper = new Epaper();
            epaper.CART_ID = "E1080";
        }


    }
}
