using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using NexenHub.Class;

namespace NexenHub.Models
{
    public class WorkOrder
    {
        public string WO_NO { get; set; }
        public string WO_STIME { get; set; }
        public string PROD_TYPE { get; set; }
        public string ITEM_ID { get; set; }
        public string ITEM_NAME { get; set; }
        public string WO_QTY { get; set; }
        public string PROD_QTY { get; set; }
        public string UNIT { get; set; }
        public string TEST_YN { get; set; }
        public string PROTOTYPE_ID { get; set; }
        public string PROTOTYPE_VER { get; set; }
        public string XCHPF { get; set; }

        public bool OE
        {
            get
            {
                bool x = this.XCHPF == "OE" ? true : false;
                return x;
            }
        }


        private GlobalDatabase dbglob = new GlobalDatabase();

        public void LoadFromMachine(string eq_id)
        {
            DataTable dt = dbglob.GetWorkOrderFromEQ(eq_id);
            if (dt.Rows.Count > 0) { 
                WO_NO = dt.Rows[0]["WO_NO"].ToString();
                WO_STIME = dt.Rows[0]["WO_STIME"].ToString();
                PROD_TYPE = dt.Rows[0]["PROD_TYPE"].ToString();
                ITEM_ID = dt.Rows[0]["ITEM_ID"].ToString();
                ITEM_NAME = dt.Rows[0]["ITEM_NAME"].ToString();
                WO_QTY = dt.Rows[0]["WO_QTY"].ToString();
                PROD_QTY = dt.Rows[0]["PROD_QTY"].ToString();
                UNIT = dt.Rows[0]["UNIT"].ToString();
                TEST_YN = dt.Rows[0]["TEST_YN"].ToString();
                PROTOTYPE_ID = dt.Rows[0]["PROTOTYPE_ID"].ToString();
                PROTOTYPE_VER = dt.Rows[0]["PROTOTYPE_VER"].ToString();

                if (TEST_YN == "N")
                    XCHPF = dt.Rows[0]["XCHPF"].ToString();
                else
                    XCHPF = "RE";
            }
        }

    }
}
