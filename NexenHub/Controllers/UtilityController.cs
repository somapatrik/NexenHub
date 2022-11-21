using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NexenHub.Class;
using System;
using System.Data;

namespace NexenHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilityController : Controller
    {

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
