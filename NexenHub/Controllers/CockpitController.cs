using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NexenHub.Class;
using NexenHub.Models;
using System.Collections.Generic;

namespace NexenHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CockpitController : ControllerBase
    {

        GlobalDatabase database = new GlobalDatabase();

        [HttpGet("inputPositions/{EQ_ID:length(5)}")]
        public ActionResult<List<EQPOS>> getInputPositions(string EQ_ID)
        {
            try 
            { 
                return database.GetInputPositions(EQ_ID);
            }
            catch 
            { 
                return StatusCode(StatusCodes.Status500InternalServerError); 
            }
        }
    }
}
