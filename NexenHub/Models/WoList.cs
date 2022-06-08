using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using NexenHub.Class;

namespace NexenHub.Models
{
    public class WoList
    {
        public string EQ_ID { get; set; }
        public List<WorkOrder> Workorders { get; set; }

        private GlobalDatabase dbglob = new GlobalDatabase();

        public WoList()
        {

        }

        public WoList(string EQID)
        {
            EQ_ID = EQID;
            LoadFromDb();
        }

        private void LoadFromDb()
        {
            if (!string.IsNullOrEmpty(EQ_ID))
            {
                Workorders = new List<WorkOrder>();

                foreach (DataRow row in dbglob.LoadWorkOrderList(EQ_ID).Rows)
                {
                    WorkOrder wo = new WorkOrder();
                    wo.WO_NO = row["WO_NO"].ToString();
                    wo.WO_STIME = row["WO_STIME"].ToString();
                    wo.WO_ETIME = row["WO_ETIME"].ToString();
                    wo.PLAN_STIME = row["PLAN_STIME"].ToString();
                    wo.PLAN_ETIME = row["PLAN_ETIME"].ToString();
                    wo.PROD_TYPE = row["PROD_TYPE"].ToString();
                    wo.ITEM_ID = row["ITEM_ID"].ToString();
                    wo.ITEM_NAME = row["ITEM_NAME"].ToString();
                    wo.WO_QTY = row["WO_QTY"].ToString();
                    wo.PROD_QTY = row["PROD_QTY"].ToString();
                    wo.UNIT = row["UNIT"].ToString();
                    wo.TEST_YN = row["TEST_YN"].ToString();
                    wo.PROTOTYPE_ID = row["PROTOTYPE_ID"].ToString();
                    wo.PROTOTYPE_VER = row["PROTOTYPE_VER"].ToString();
                    wo.DEL_FLAG = row["DEL_FLAG"].ToString();
                    wo.WO_PROC_STATE = row["WO_PROC_STATE"].ToString(); 
                    Workorders.Add(wo);
                }
            }
        }


    }
}
