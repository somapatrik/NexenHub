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
    public class LotController : ControllerBase
    {
        [HttpGet("lothis/{lotid}")]
        public ActionResult<List<LotHisItem>> Get(string lotid)
        {
            if (lotid.Length == 15)
            {
                LotItem lot = new LotItem(lotid);
                lot.LoadHistory();
                List<LotHisItem> history = new List<LotHisItem>();

                foreach (LotHisItem his in lot.History)
                {
                    // Don´t care about repeating history. HISTORY SHOULDN´T REPEAT ITSELF
                    if (history.Find(x=> x.itemState == his.itemState && 
                                         x.locationName == his.locationName &&
                                         x.lotState == his.lotState &&
                                         x.qtyUnit == his.qtyUnit) == null)
                    {
                        history.Add(his);
                    }
                }

                return history;
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
