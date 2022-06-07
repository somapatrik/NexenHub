using NexenHub.Class;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Models
{
    public class WorkorderPlanTBM
    {
        public List<WorkorderPlanItem> plannedWorkorders { get; set; }

        public string EQ_ID { get; set; }

        public int PlanSummary { 
            get 
            {
                if (plannedWorkorders != null)
                    return plannedWorkorders.Select(x => x.FINAL_PLAN).Sum();
                else
                    return 0;
            } 
        }

        private GlobalDatabase dbglob = new GlobalDatabase();

        public WorkorderPlanTBM(string EQ_ID)
        {
            this.EQ_ID = EQ_ID;
            LoadPlan();
        }

        private void LoadPlan()
        {
            plannedWorkorders = new List<WorkorderPlanItem>();
            foreach (DataRow row in dbglob.GetTbmPlan(EQ_ID).Rows)
                plannedWorkorders.Add(new WorkorderPlanItem()
                {
                    WO_NO = row["WO_NO"].ToString(),
                    PLAN_START = DateTime.Parse(row["PLAN_START"].ToString()),
                    PLAN_STOP = DateTime.Parse(row["PLAN_STOP"].ToString()),
                    SECONDS_FULL = decimal.Parse(row["SECONDS_FULL"].ToString()),
                    CYCLE_TIME = int.Parse(row["CYCLE_TIME"].ToString()),
                    QTY_FULL = int.Parse(row["QTY_FULL"].ToString()),
                    IDEAL_START = DateTime.Parse(row["IDEAL_START"].ToString()),
                    IDEAL_END = DateTime.Parse(row["IDEAL_END"].ToString()),
                    SECONDS_IDEAL = decimal.Parse(row["SECONDS_IDEAL"].ToString()),
                    QTY_IDEAL = int.Parse(row["QTY_IDEAL"].ToString()),
                    PREV_PROD = int.Parse(row["PREV_PROD"].ToString()),
                    FINAL_PLAN = int.Parse(row["FINAL_PLAN"].ToString())
                });
        }

    }
}
