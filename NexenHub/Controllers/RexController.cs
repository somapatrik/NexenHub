using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NexenHub.Class;
using NexenHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RexController : ControllerBase
    {
        RexVersion rexver = new RexVersion();
        GlobalDatabase dbglob = new GlobalDatabase();


        [HttpGet("version")]
        public ActionResult<RexVersion> Get()
        {
            return rexver;
        }

        [HttpPost("latest")]
        public ActionResult<DateTime> PostLatest()
        {
            HttpRequest request = HttpContext.Request;

            string ip = request.Headers.ContainsKey("ip") ? request.Headers["ip"].ToString() : "";
            string versionName = request.Headers.ContainsKey("versionName") ? request.Headers["versionName"].ToString() : "";

            AppVersionRex version = new AppVersionRex();
            return version.VersionDate;
        }

        [HttpGet("lotitem/{code}")]
        public ActionResult<LotItem> Get(string code)
        {
            string lotid = "";

            if (code.Length == 5)
                lotid = dbglob.Cart2Lot(code);
            else if (code.Length == 15)
                lotid = code;

            if (lotid.Length == 15)
            {
                LotItem lot = new LotItem(lotid);
                lot.LoadHistoryClean();
                lot.RemoveUselessHistory();

                return lot;
            }
            else
            {
                return BadRequest();
            }
        }


    }
}
