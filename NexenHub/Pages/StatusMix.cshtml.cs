using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexenHub.Class;

namespace NexenHub.Pages
{
    public class StatusMixModel : PageModel
    {
        GlobalDatabase dbglob = new GlobalDatabase();
        public List<StatusEq> statuses;

        public void OnGet()
        {
            DataTable dt = dbglob.GetDashboardStatus("M");

            statuses = new List<StatusEq>();

            foreach (DataRow row in dt.Rows)
            {
                StatusEq eq = new StatusEq();
                eq.ID = row["EQ_ID"].ToString();
                eq.Name = row["EQ_NAME"].ToString();
                eq.Downtime = row["NON_NAME"].ToString();
                eq.Item_id = row["ITEM_ID"].ToString();
                eq.BgColor = row["BGCOLOR"].ToString();
                eq.FrColor = row["FRCOLOR"].ToString();
                statuses.Add(eq);
            }
        }
    }
}
