using Microsoft.AspNetCore.Mvc;
using NexenHub.Class;
using NexenHub.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexenHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdController : ControllerBase
    {
        [HttpGet("{proc}")]
        public async Task<ActionResult<string>> Get(string proc)
        {
            try
            {
                if (!string.IsNullOrEmpty(proc)) 
                { 
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("SELECT /*+INDEX(TB_PR_M_PROD IX_PR_M_PROD_5)*/");
                    query.AppendLine("SUM(PROD_QTY)");
                    query.AppendLine("FROM TB_PR_M_PROD");
                    query.AppendLine("WHERE PROD_DATE = (SELECT TO_CHAR(SYSDATE - 6/24, 'YYYYMMDD') FROM DUAL)");
                    query.AppendLine("AND PLANT_ID = 'P500'");
                    query.AppendLine("AND WC_ID = :proc");
                    query.AppendLine("AND USE_YN = 'Y'");
                    query.AppendLine("AND SHIFT IS NOT NULL");

                    DBOra db = new DBOra(query.ToString());
                    db.AddParameter("proc", proc.ToUpper(), Oracle.ManagedDataAccess.Client.OracleDbType.NVarchar2);
                    DataTable dt = db.ExecTable();

                    if (dt.Rows.Count > 0)
                        return dt.Rows[0][0].ToString();
                    else
                        return "";

                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "error";
            }
        }
    }
}
