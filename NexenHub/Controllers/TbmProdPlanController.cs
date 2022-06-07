using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NexenHub.Models;

namespace NexenHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TbmProdPlanController : ControllerBase
    {

        List<string> Ids = new List<string>() { "10033", "10034", "10035", "10036", "10037", "10038", "10039", "10040", "10041", "10042", "10043", "10044" };

        public async Task<ActionResult<List<WorkorderPlanTBM>>> Get()
        {
            
            List<WorkorderPlanTBM> planList = new List<WorkorderPlanTBM>();

            await Task.Run(() => {
                Ids.ForEach(id => planList.Add(new WorkorderPlanTBM(id)));
            });

            return planList;
        }

        [HttpGet("{eqid}")]
        public async Task<ActionResult<WorkorderPlanTBM>> Get(string eqid)
        {
            if (Ids.Contains(eqid))
                return new WorkorderPlanTBM(eqid);
            else
                return BadRequest();
        }
    }
}
