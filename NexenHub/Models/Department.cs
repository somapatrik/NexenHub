using System.Collections.Generic;

namespace NexenHub.Models
{
    public class Department
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string UpDepartment_ID { get; set; }

        public int Level { get; set; }
    }
}
