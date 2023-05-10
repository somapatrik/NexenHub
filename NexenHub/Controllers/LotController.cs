using Microsoft.AspNetCore.Mvc;
using NexenHub.Models;
using NexenHub.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NexenHub.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class LotController : ControllerBase
    {
        [HttpGet("lothis/{lotid}")]
        public ActionResult<List<LotHisItem>> Get(string lotid)
        {
            if (lotid.Length == 15)
            {
                LotItem lot = new LotItem(lotid);
                lot.LoadHistory();
                lot.RemoveUselessHistory();

                return lot.History;
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("topdf")]
        public ActionResult ConvertToPDF()
        {
            return null;
        }



    }
}
