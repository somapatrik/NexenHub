using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Class
{
    public class StatusEq
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Downtime { get; set; }
        public string Item_id { get; set; }
        public string BgColor { get; set; }
        public string FrColor { get; set; }
        public string formatTime { get => Start.ToString("HH:mm"); }
        public DateTime Start { get; set; }
    }

}
