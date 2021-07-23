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
    public class EslController : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<Esl>> Get(string id)
        {
            Esl epaper = new Esl();

            if (id.Length == 5 || id.Length ==15)
            {
                await Task.Run(() =>
                {
                    if (id.Length == 5)
                        epaper.CART_ID = id;
                    else if (id.Length == 15)
                        epaper.LOT_ID = id;
                });

                return epaper;
            }
            else
            {
                return BadRequest();
            }

        }
    }
}
