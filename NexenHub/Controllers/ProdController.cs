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

        GlobalDatabase dbglob = new GlobalDatabase();

        [HttpGet("{proc}")]
        public ActionResult<string> GetProd(string proc)
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

        [HttpGet("MonthProdPlan")]
        public ActionResult<List<int>> GetProdPlan()
        {
            return dbglob.GetTBMMonthPlan();
        }


        [HttpGet("PM")]
        public ActionResult<List<StatusEq>> GetPM()
        {
            List<StatusEq> MachinesPM = new List<StatusEq>();

            foreach (DataRow row in dbglob.GetDashboardStatus("").Rows)
            {
                if (row["NONWRK_CODE"].ToString() == "N016" && row["WC_ID"].ToString() != "U")
                    MachinesPM.Add(new StatusEq()
                    {
                        Name = row["EQ_NAME"].ToString(),
                        Downtime = row["NON_NAME"].ToString(),
                        Start = DateTime.Parse(row["STIME"].ToString())
                    });
            }

            return MachinesPM;
        }



    }
}
