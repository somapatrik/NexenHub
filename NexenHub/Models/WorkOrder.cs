using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using NexenHub.Class;

namespace NexenHub.Models
{
    public class WorkOrder
    {
        public string WO_NO { get; set; }
        public string WO_STIME { get; set; }
        public string WO_ETIME { get; set; }
        public string PLAN_STIME { get; set; }
        public string PLAN_ETIME { get; set; }
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
        public string WO_PROC_STATE { get; set; }
        public string OP_PROC_STATE { get; set; }
        public string OP_PROD_STIME { get; set; }
        public string OP_PROD_ETIME { get; set; }
        public string WU_PROC_STATE { get; set; }
        public string WU_PROD_STIME { get; set; }
        public string WU_PROD_ETIME { get; set; }
        public string DEL_FLAG { get; set; }
        public string USE_YN { get; set; }
        public bool OE
        {
            get
            {
                bool x = this.XCHPF == "OE";
                return x;
            }
        }
        public bool WO_EXISTS { get; set; }
        public string WO_ChildItemID { get; set; }
        public string WO_ChildItemName { get; set; }
        public string WO_ChildItemCompound { get; set; }

        public string WC_ID { get; set; }
        public string PROC_ID { get; set; }
        public string EQ_ID { get; set; }

        public DateTime STIME_DATE { 
            get 
            {
                return string.IsNullOrEmpty(WO_STIME) ? DateTime.MinValue : DateTime.Parse(WO_STIME);
            } 
        }

        public DateTime ETIME_DATE {
            get
            {
                return string.IsNullOrEmpty(WO_ETIME) ? DateTime.MinValue : DateTime.Parse(WO_ETIME);
            }
        }

        public DateTime PLAN_STIME_DATE {
            get
            {
                return string.IsNullOrEmpty(PLAN_STIME) ? DateTime.MinValue : DateTime.Parse(PLAN_STIME);
            }
        }

        public DateTime PLAN_ETIME_DATE {
            get
            {
                return string.IsNullOrEmpty(PLAN_ETIME) ? DateTime.MinValue : DateTime.Parse(PLAN_ETIME);
            }
        }


        public string FORMAT_WO_STATE => FormatState(WO_PROC_STATE); 
        public string FORMAT_WU_STATE => FormatState(WU_PROC_STATE);
        public string FORMAT_OP_STATE => FormatState(OP_PROC_STATE);

        private string FormatState(string msg)
        {
            switch (msg)
            {
                case "S":
                    return "Running";
                case "W":
                    return "Waiting";
                case "F":
                    return "Completed";
                default:
                    return msg;
            }
        }

        private GlobalDatabase dbglob = new GlobalDatabase();

        public void LoadFromMachine(string eq_id)
        {
            WO_EXISTS = false;
            DataTable dt = dbglob.GetWorkOrderFromEQ(eq_id);
            if (dt.Rows.Count > 0) 
            {
                WO_EXISTS = true;
                WO_NO = dt.Rows[0]["WO_NO"].ToString();
                WO_STIME = dt.Rows[0]["WO_STIME"].ToString();
                PROD_TYPE = dt.Rows[0]["PROD_TYPE"].ToString();
                ITEM_ID = dt.Rows[0]["ITEM_ID"].ToString();
                ITEM_NAME = dt.Rows[0]["ITEM_NAME"].ToString();
                WO_QTY = dt.Rows[0]["WO_QTY"].ToString();
                PROD_QTY = dt.Rows[0]["PROD_QTY"].ToString();
                UNIT = dt.Rows[0]["UNIT"].ToString();

                // This is not a mistake
                if (dt.Rows[0]["TEST_YN"].ToString() == "Y")
                    TEST_YN = "Y";
                else
                    TEST_YN = "N";

                PROTOTYPE_ID = dt.Rows[0]["PROTOTYPE_ID"].ToString();
                PROTOTYPE_VER = dt.Rows[0]["PROTOTYPE_VER"].ToString();

                if (TEST_YN == "N")
                    XCHPF = dt.Rows[0]["XCHPF"].ToString();
                else
                    XCHPF = "RE";
            }
        }

        /// <summary>
        /// Gets WO like ICS, only loads data for a mobile test
        /// </summary>
        /// <param name="eq_id"></param>
        public void LoadLikeICS(string eq_id, string MiniPC)
        {
            WO_EXISTS = false;
            DataTable dt = dbglob.LoadWorkOrder(eq_id);
            if (dt.Rows.Count > 0)
            {
                WO_EXISTS = true;
                WO_NO = dt.Rows[0]["WO_NO"].ToString();
                ITEM_ID = dt.Rows[0]["ITEM_ID"].ToString();
                PROD_TYPE = dt.Rows[0]["PROD_TYPE"].ToString();
                PROD_QTY = dt.Rows[0]["PROD_QTY"].ToString();
                WO_QTY = dt.Rows[0]["WO_QTY"].ToString();

                // Load child item
                DataTable dx = dbglob.LoadWOChildItem(ITEM_ID, MiniPC);
                if (dx.Rows.Count > 0)
                {
                    WO_ChildItemID = dx.Rows[0]["ITEM_ID"].ToString();
                    WO_ChildItemName = dx.Rows[0]["ITEM_NAME"].ToString();
                    WO_ChildItemCompound = dx.Rows[0]["PLAN_COMPOUND"].ToString();
                }

            }
        }

        public void LoadById(string WONO)
        {
            WO_EXISTS = false;
            if (WONO.Length == 15)
            {
                WO_NO = WONO;
                DataTable dt = dbglob.LoadWorkOrderByWo(WONO);
                if (dt.Rows.Count > 0)
                {
                    WO_EXISTS = true;
                    WO_NO = dt.Rows[0]["WO_NO"].ToString();
                    WO_STIME = dt.Rows[0]["WO_STIME"].ToString();
                    WO_ETIME = dt.Rows[0]["WO_ETIME"].ToString();
                    PLAN_STIME = dt.Rows[0]["PLAN_STIME"].ToString();
                    PLAN_ETIME = dt.Rows[0]["PLAN_ETIME"].ToString();
                    PROD_TYPE = dt.Rows[0]["PROD_TYPE"].ToString();
                    ITEM_ID = dt.Rows[0]["ITEM_ID"].ToString();
                    WO_QTY = dt.Rows[0]["WO_QTY"].ToString();
                    PROD_QTY = dt.Rows[0]["PROD_QTY"].ToString();
                    UNIT = dt.Rows[0]["UNIT"].ToString();
                    TEST_YN = dt.Rows[0]["TEST_YN"].ToString();
                    PROTOTYPE_ID = dt.Rows[0]["PROTOTYPE_ID"].ToString();
                    PROTOTYPE_VER = dt.Rows[0]["PROTOTYPE_VER"].ToString();
                    DEL_FLAG = dt.Rows[0]["DEL_FLAG"].ToString();
                    WO_PROC_STATE = dt.Rows[0]["WO_PROC_STATE"].ToString();
                    WU_PROC_STATE = dt.Rows[0]["WU_PROC_STATE"].ToString();
                    OP_PROC_STATE = dt.Rows[0]["OP_PROC_STATE"].ToString();
                    OP_PROD_STIME = dt.Rows[0]["OP_PROD_STIME"].ToString();
                    OP_PROD_ETIME = dt.Rows[0]["OP_PROD_ETIME"].ToString();
                    WU_PROD_STIME = dt.Rows[0]["WU_PROD_STIME"].ToString();
                    WU_PROD_ETIME = dt.Rows[0]["WU_PROD_ETIME"].ToString();
                    PROC_ID = dt.Rows[0]["PROC_ID"].ToString();
                    WC_ID = dt.Rows[0]["WC_ID"].ToString();
                    EQ_ID = dt.Rows[0]["EQ_ID"].ToString();

                }
            }

        }
    }
}
