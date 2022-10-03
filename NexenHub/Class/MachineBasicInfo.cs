using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Class
{
    public class MachineBasicInfo
    {
        public string Name { get; set; }
        public string EQ_ID { get; set; }
        public string WC_ID { get; set; }
        public string PROC_ID { get; set; }

        private GlobalDatabase db = new GlobalDatabase();
        public MachineBasicInfo(string EQID)
        {
            EQ_ID = EQID;
            DataTable dt = db.GetMachineList(EQ_ID);
            if (dt.Rows.Count > 0)
            {
                Name = dt.Rows[0]["Name"].ToString();
                WC_ID = dt.Rows[0]["WC_ID"].ToString();
                PROC_ID = dt.Rows[0]["PROC_ID"].ToString();
            }
        }
    }
}
