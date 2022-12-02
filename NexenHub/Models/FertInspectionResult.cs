using System;

namespace NexenHub.Models
{
    public class FertInspectionResult
    {
        public int SEQ { get; set; }
        public string PROC { get; set; }
        public string PROC_FULLNAME { get; set; }
        public DateTime InspectionTime { get; set; }
        public string SHIFT { get; set; }
        public string SHIFT_RBG { get; set; }
        public string BAD_ID { get; set; }
        public string BAD_GRADE { get; set; }
        public string LOC_MOLD { get; set; }
        public string LOC_SIDE { get; set; }
        public string LOC_ZONE { get; set; }
        public string LOC_POSITION { get; set; }
        public string CQ2 { get; set; }
        public string UserName { get; set; }
    }
}
