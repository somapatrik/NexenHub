using Microsoft.AspNetCore.Mvc;
using NexenHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NexenHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TireInspectionController : ControllerBase
    {
        [HttpGet("{code}")]
        public async Task<ActionResult<TireInspection>> Get(string code)
        {
            TireInspection tireInspection;
            tireInspection = new TireInspection(code);
            return tireInspection;
        }
    }
}
