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
using System.Text;
using Newtonsoft.Json; 

namespace NexenHub.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public YearProd yearProd;

        // private GlobalDatabase GlobalDb = new GlobalDatabase();

        public void OnGet()
        {
            yearProd = new YearProd();
        }

        
    }
}
