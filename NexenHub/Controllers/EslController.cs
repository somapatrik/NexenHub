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

            if (id.Length != 5)
            {
                return BadRequest();
            }
            else
            {
                await Task.Run(() =>
                {
                    epaper.CART_ID = id.ToUpper();
                });

                return epaper;
            }

        }
    }
}
