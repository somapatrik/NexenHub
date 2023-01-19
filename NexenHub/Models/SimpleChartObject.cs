using System.Collections.Generic;

namespace NexenHub.Models
{
    public class SimpleChartObject
    {
        public string ChartTitle { get; set; }
        public List<string> Labels { get; set; }
        public List<string> Values { get; set; }

        public SimpleChartObject()
        {
            ChartTitle = "";
            Labels = new List<string>();
            Values = new List<string>();
        }

    }
}
