using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Models
{
    public class LotHisItem
    {
        public DateTime transDate { get; set; }
        public string locationName { get; set; }
        public string lotState { get; set; }
        public string itemState { get; set; }
        public string qtyUnit { get; set; }

    }
}
