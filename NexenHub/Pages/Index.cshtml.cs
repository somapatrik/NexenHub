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

namespace NexenHub.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        public string Production { get; set; }
                
        public void OnGet()
        { 
            DBOra db = new DBOra("select count(*) from TB_PR_M_PROD where WC_ID = 'U' and PLANT_ID = 'P500' and SHIFT is not null and to_char(SYSDATE, 'YYYYMMDD') = PROD_DATE");
            try
            {
                DataTable dt = db.ExecTable();
                Production = "Today´s production: " + dt.Rows[0][0].ToString();

            } 
            catch (Exception ex)
            {
                Production = "Unable to load from DB";
            }
        }


    }
}
