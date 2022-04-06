using Microsoft.AspNetCore.Http;
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

    public class ValidController : Controller
    {
        [HttpGet("inputpos")]
        public async Task<ActionResult<List<InputPosition>>> GetInputPos()
        {
            List<InputPosition> postitions = new List<InputPosition>();

            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT EQ.EQ_ID, EQ.EQ_NAME, POS.IO_POSID ");
            query.AppendLine("FROM TB_EQ_M_EQPOS POS");
            query.AppendLine("JOIN TB_EQ_M_EQUIP EQ on EQ.EQ_ID = POS.EQ_ID");
            query.AppendLine("WHERE EQ.USE_YN = 'Y'");
            query.AppendLine("AND POS.USE_YN = 'Y'");
            query.AppendLine("AND POS.IO_POSID like 'H%'");
            query.AppendLine("ORDER BY EQ_ID, IO_POSID");

            DBOra db = new DBOra(query.ToString());
            DataTable dt = new DataTable();
            await Task.Run(() => { dt = db.ExecTable(); });

            foreach (DataRow row in dt.Rows)
                postitions.Add(new InputPosition()
                {
                    EQ_ID = row["EQ_ID"].ToString(),
                    MachineName = row["EQ_NAME"].ToString(),
                    POS_ID = row["IO_POSID"].ToString()
                });

            return postitions;
        }

        [HttpGet("{lot}/{eq}/{pos}")]
        public async Task<ActionResult<InputCheck>> GetValidResult(string lot, string eq, string pos)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT MINIPC_ID");
            query.AppendLine("FROM TB_CM_M_MONITORING_READER");
            query.AppendLine("WHERE POSID = :pos");
            query.AppendLine("AND EQ_ID = :eq");

            DBOra db = new DBOra(query.ToString());
            db.AddParameter("pos", pos, Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2);
            db.AddParameter("eq", eq, Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2);

            DataTable dt = db.ExecTable();
            if (dt.Rows.Count > 0)
            {
                string minipc = db.ExecTable().Rows[0][0].ToString();
                InputCheck check = new InputCheck(lot, eq, pos);
                return check;
            }
            else
            {
                return BadRequest();
            }

            
        }

        public class InputPosition
        {
            public string EQ_ID { get; set; }
            public string MachineName { get; set; }
            public string POS_ID { get; set; }
        }
    }
}
