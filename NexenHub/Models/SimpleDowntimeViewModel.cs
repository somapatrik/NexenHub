using NexenHub.Class;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Models
{
    public class SimpleDowntimeViewModel
    {
        string EQ_ID { get; set; }
        Shift Shift { get; set; }

        List<DownTimeItem> downTimes;

        GlobalDatabase dbglob = new GlobalDatabase();
        public SimpleDowntimeViewModel(string EQID)
        {
            EQ_ID = EQID;
            Shift = new Shift();

            LoadDownTimes();
        }

        public void LoadDownTimes()
        {
            downTimes = new List<DownTimeItem>();

            using (DataTable dt = dbglob.GetShiftDownTimes(EQ_ID, Shift.STIME, Shift.ETIME))
            {
                foreach(DataRow row in dt.Rows)
                {
                    downTimes.Add(new DownTimeItem()
                    {
                        STIME = DateTime.Parse(row["STIME"].ToString()),
                        ETIME = DateTime.Parse(row["ETIME"].ToString()),
                        CODE = row["NONWRK_CODE"].ToString(),
                        NAME = row["NONWRK_NAME"].ToString()
                    }) ;
                }
            }
        }


    }

    public class DownTimeItem
    {
        public DateTime STIME;
        public DateTime ETIME;
        public string CODE;
        public string NAME;
    }
}
