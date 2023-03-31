using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
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

        [HttpGet("CurrentProd/{proc}")]
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

        [HttpGet("TBMPlanCurrent")]
        public ActionResult<int> GetTBMPlanNow()
        {
            int prodDay = DateTime.Now.AddHours(-6).Day;
            List<int> planDays = dbglob.GetTBMMonthPlan();
            return planDays[prodDay - 1];

        }

        [HttpGet("CurePlanCurrent")]
        public ActionResult<int> GetCurePlanNow()
        {
            int prodDay = DateTime.Now.AddHours(-6).Day;
            List<int> planDays = dbglob.GetCUREMonthPlan();
            return planDays[prodDay - 1];

        }

        [HttpGet("SWVersions")]
        public ActionResult<string[]> GetSWVersions()
        {
            int ICSCount;
            int LatestICSCount = 0;
            DateTime LatestICSVersion = DateTime.MinValue;

            int RexCount;
            int LatestCountRex = 0;
            DateTime LatestRexVersion = DateTime.MinValue; ;

            int IOCount;
            int LatestCountIO = 0;
            DateTime LatestIOVersion = DateTime.MinValue; ;

            DataTable dt = dbglob.GetLatestSoftwareCount("ics");
            if (dt.Rows.Count > 0)
            {
                LatestICSCount = int.Parse(dt.Rows[0][0].ToString());
                LatestICSVersion = DateTime.Parse(dt.Rows[0][1].ToString());
                
            }

            dt = dbglob.GetLatestSoftwareCount("rex");
            if (dt.Rows.Count > 0)
            {
                LatestCountRex = int.Parse(dt.Rows[0][0].ToString());
                LatestRexVersion = DateTime.Parse(dt.Rows[0][1].ToString());
                
            }

            dt = dbglob.GetLatestSoftwareCount("ioserver");
            if (dt.Rows.Count > 0)
            {
                LatestCountIO = int.Parse(dt.Rows[0][0].ToString());
                LatestIOVersion = DateTime.Parse(dt.Rows[0][1].ToString());
            }

            RexCount = dbglob.GetRexCount();
            ICSCount = dbglob.GetICSCount();
            IOCount = dbglob.GetIOServerCount();

            string[] versionArray = { 
                LatestICSVersion.ToString("yy-MM-dd"), LatestICSCount.ToString(), ICSCount.ToString(),
                LatestIOVersion.ToString("yy-MM-dd"), LatestCountIO.ToString(), IOCount.ToString(),
                LatestRexVersion.ToString("yy-MM-dd"), LatestCountRex.ToString(), RexCount.ToString(),
            };
            return versionArray;
        }

        [HttpGet("OldVersions")]
        public ActionResult<object> GetOldVersion()
        {
            DataTable dt = dbglob.GetOldVersions();
            List<object> Old = new List<object>();
            foreach(DataRow row in dt.Rows)
            {
                Old.Add(new { Software=row["SOFTWARE"].ToString(),Name = row["DEVICE"].ToString(), IP = row["IP"].ToString() });
            }

            return Old;
        }

        [HttpGet("Unreachable")]
        public ActionResult<List<PingDevice>> GetUnReachAble()
        {
            List<PingDevice> pingDevices = new List<PingDevice>();
            foreach (DataRow row in dbglob.GetPingDevices().Rows)
                pingDevices.Add(new PingDevice() { Name = row["DISPLAYNAME"].ToString(), IP = row["IP"].ToString() });

            return pingDevices.FindAll(d => !d.PingResult);
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

        [HttpGet("actDowntime/{eqid}")]
        public ActionResult<object> GetActDownTime(string eqid)
        {
            
            DataTable dt = dbglob.GetActNonWrk(eqid);
            var ret = new {Code = "", Description = "" };
            string Code = "";
            string Description = "";

            if (dt.Rows.Count > 0)
            {
                Code = dt.Rows[0]["NON_CODE"].ToString();
                Description = dt.Rows[0]["NON_NAME"].ToString();
            }
            return new {Code = Code, Description = Description};
        }

        [HttpGet("WorkSectionOEE/{wcid}/{fact?}")]
        public ActionResult<WorkSectionOee> GetWorkSectionOee(string wcid, string fact="")
        {
            return new WorkSectionOee(wcid,fact);
        }

    }
}
