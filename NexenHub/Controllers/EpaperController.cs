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
    public class EpaperController : ControllerBase
    {

        [HttpGet("{id}")]
        public ActionResult<Epaper> Get(string id)
        {
            Epaper epaper = new Epaper(id.ToUpper());
            return epaper;
        }

    }
}
