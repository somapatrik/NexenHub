using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("version")]
        public ActionResult<RexVersion> Get()
        {
            return rexver;
        }



    }
}
