using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        GlobalDatabase dbglob = new GlobalDatabase();

        [HttpGet("version")]
        public ActionResult<RexVersion> GetVersion()
        {
            RexVersion rexver = new RexVersion();
            return rexver;
        }

        [HttpGet("latestversion")]
        public ActionResult<DateTime> GetLatestVersion()
        {
            AppVersionRex version = new AppVersionRex();
            return version.VersionDate;
        }

        // TODO: Can be deleted if every rex is > 22-11-30
        [HttpPost("reportversion")]
        public ActionResult PostReportVersion()
        {
            try 
            { 
                HttpRequest request = HttpContext.Request;

                string ip = request.Headers.ContainsKey("ip") ? request.Headers["ip"].ToString() : "";
                string versionName = request.Headers.ContainsKey("versionName") ? request.Headers["versionName"].ToString() : "";
                string appId = request.Headers.ContainsKey("appId") ? request.Headers["appId"].ToString() : "";

                dbglob.UpdateVersion(appId, ip, versionName);

            }
            catch
            {
                return BadRequest();
            }

            return Ok();
        }


        [HttpPost("reportSoftware")]
        public ActionResult PostReportSoftware()
        {
            try
            {
                var httpRequest = HttpContext.Request;

                string ip = httpRequest.Form["ip"].ToString();
                string versionName = httpRequest.Form["versionName"].ToString();
                string appId = httpRequest.Form["appId"].ToString();

                dbglob.UpdateVersion(appId, ip, versionName);

            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok();
        }

        [HttpGet("lotitem/{code}")]
        public ActionResult<LotItem> GetLotItem(string code)
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


        [HttpGet("commongt/{lotid}")]
        public ActionResult<List<string>> GetCommonGt(string lotid)
        {
            return dbglob.GetCommonGt(lotid);
        }


    }
}
