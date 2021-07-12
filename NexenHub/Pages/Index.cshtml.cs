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

namespace NexenHub.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public string ProductionGT { get; set; }

        public string ProductionTire { get; set; }
        public string MaxValue { get; set; }
        public string ActValueGT { get; set; }

        public string ActValueTire { get; set; }


        public void OnGet()
        {
            MaxValue = "15000";
            GetProdGT();
            GetProdTire();
        }

        public void GetProdGT()
        {
            StringBuilder query = new StringBuilder();
            query.Append("select /*+index (tb_pr_m_prod IX_PR_M_PROD_5)*/ count(*)");
            query.AppendLine("from TB_PR_M_PROD");
            query.AppendLine("where WC_ID = 'T'");
            query.AppendLine("AND USE_YN = 'Y'");
            query.AppendLine("AND PLANT_ID = 'P500'");
            query.AppendLine("AND SHIFT is not null");
            query.AppendLine("AND PROD_DATE = (select /*+index (tb_pr_m_prod IX_PR_M_PROD_5)*/  max(prod_date) from TB_PR_M_PROD where plant_id = 'P500' and shift is not null and wc_id is not null)");

            DBOra db = new DBOra(query.ToString()) ;
            DataTable dt = db.ExecTable();
            if (dt.Rows.Count > 0)
            {
                ProductionGT = dt.Rows[0][0].ToString();
                ActValueGT = GetProcGT(ProductionGT);
            }
                
        }

        public void GetProdTire()
        {
            StringBuilder query = new StringBuilder();
            query.Append("select /*+index (tb_pr_m_prod IX_PR_M_PROD_5)*/ count(*)");
            query.AppendLine("from TB_PR_M_PROD");
            query.AppendLine("where WC_ID = 'U'");
            query.AppendLine("AND USE_YN = 'Y'");
            query.AppendLine("AND PLANT_ID = 'P500'");
            query.AppendLine("AND SHIFT is not null");
            query.AppendLine("AND PROD_DATE = (select /*+index (tb_pr_m_prod IX_PR_M_PROD_5)*/  max(prod_date) from TB_PR_M_PROD where plant_id = 'P500' and shift is not null and wc_id is not null)");

            DBOra db = new DBOra(query.ToString());
            DataTable dt = db.ExecTable();
            if (dt.Rows.Count > 0)
            {
                ProductionTire = dt.Rows[0][0].ToString();
                ActValueTire = GetProcGT(ProductionTire);
            }

        }

        public string GetProcGT(string actprod)
        {
            double max = double.Parse(MaxValue);
            double prod = double.Parse(actprod);

            double proc = (prod/max) * 100;
            string final = Math.Round(proc).ToString();

            return final;
        }


    }
}
