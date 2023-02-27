using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NexenHub.Class;
using NexenHub.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace NexenHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilityController : Controller
    {
        GlobalDatabase dbglob = new GlobalDatabase();


        [HttpGet("MemberByDept/{deptID:minlength(8):maxlength(8)}")]
        public ActionResult<List<SimpleMember>> GetMemberByDept(string deptID)
        {
            List<SimpleMember> members = new List<SimpleMember>();
            DataTable dt = dbglob.MembersByDepartment(deptID);
            foreach (DataRow row in dt.Rows)
            {
                members.Add(new SimpleMember()
                {
                     ID = row["MEMBER_ID"].ToString(),
                     DEPT_ID = row["DEPT_ID"].ToString(),
                     NAME = row["MEMBER_NAME"].ToString(),
                     EMAIL = row["EMAIL"].ToString(),
                     POSITION = row["POSITION"].ToString()
                });
            }
            return members;
        }

        public class SimpleMember
        {
            public string ID { get; set; }
            public string DEPT_ID { get; set; }
            public string NAME { get; set; }
            public string EMAIL { get; set; }
            public string POSITION { get; set; }
        }

        [HttpGet("APIStatus")]
        public ActionResult<APIStatus> GetApiStatus()
        {
            return new APIStatus();
        }

        public class APIStatus
        {
            public bool API { get; set; }
            public bool DB { get; set; }

            public APIStatus()
            {
                API = true;
                TestDB();
            }

            private void TestDB()
            {
                try 
                { 
                    DBOra db = new DBOra("SELECT 1 FROM DUAL");
                    DB = db.ExecTable().Rows[0][0].ToString() == "1";
                }
                catch{}
            }
        }

    }
}
