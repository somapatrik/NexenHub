using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Class
{
    public class WorkorderPlanItem
    {
            public string WO_NO { get; set; }
            public DateTime PLAN_START { get; set; }
            public DateTime PLAN_STOP { get; set; }
            public decimal SECONDS_FULL { get; set; }
            public int CYCLE_TIME { get; set; }
            public int QTY_FULL { get; set; }
            public DateTime IDEAL_START { get; set; }
            public DateTime IDEAL_END { get; set; }
            public decimal SECONDS_IDEAL { get; set; }
            public int QTY_IDEAL { get; set; }
            public int PREV_PROD { get; set; }
            public int FINAL_PLAN { get; set; }

    }
}
