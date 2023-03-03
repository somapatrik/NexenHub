using Microsoft.AspNetCore.Mvc;
using NexenHub.Class;
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
    public class ForecastController : ControllerBase
    {
        [HttpGet("{proc}")]
        public async Task<ActionResult<string>> Get(string proc)
        {
            try
            {
                if (proc.ToUpper() == "T" || proc.ToUpper() == "U")
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("SELECT FN_PR_M_PROD_FORECAST('P500','NEX1',:proc) FROM DUAL");

                    DBOra db = new DBOra(query.ToString());
                    db.AddParameter("proc", proc.ToUpper(), Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2);
                    DataTable dt = db.ExecTable();

                    if (dt.Rows.Count > 0)
                        return dt.Rows[0][0].ToString();
                    else
                        return "";

                }
                else
                {
                    return "HOVNO";
                }
            }
            catch
            {
                return "error";
            }
        }
    }
}
