using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Class
{
    public class Shift
    {

        public string SHIFT { get; set; }
        public string SHIFT_RGB { get; set; }
        public DateTime STIME { get; set; }
        public DateTime ETIME { get; set; }
        public int REAL_WORK_SEC { get; set; }
        public int WEEK { get; set; }


        private GlobalDatabase dbglob = new GlobalDatabase();
        public Shift()
        {
            using (DataTable dt = dbglob.GetShiftInfo())
            {
                if (dt.Rows.Count > 0)
                {
                    SHIFT = dt.Rows[0]["SHIFT"].ToString();
                    SHIFT_RGB = dt.Rows[0]["SHIFT_RGB"].ToString();
                    STIME = DateTime.Parse(dt.Rows[0]["STIME"].ToString());
                    ETIME = DateTime.Parse(dt.Rows[0]["ETIME"].ToString());
                    REAL_WORK_SEC = int.Parse(dt.Rows[0]["REAL_WORK_SEC"].ToString());
                    WEEK = int.Parse(dt.Rows[0]["W"].ToString());
                }
            }

        }
    }
}
