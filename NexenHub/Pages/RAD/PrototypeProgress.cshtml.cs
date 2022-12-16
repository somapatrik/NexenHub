using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexenHub.Class;
using System.Collections.Generic;
using System.Data;

namespace NexenHub.Pages.RAD
{
    public class PrototypeProgressModel : PageModel
    {
        GlobalDatabase dbglob = new GlobalDatabase();

        public List<string> xLegend = new List<string>();
        public List<string> yREQ = new List<string>();
        public List<string> yTBM = new List<string>();
        public List<string> yCUR = new List<string>();

        public void OnGet()
        {
            DataTable dt = dbglob.GetPrototypeProgressChart();
            foreach (DataRow r in dt.Rows)
            {
                xLegend.Add(r["EMR_ID"].ToString());
                yREQ.Add(r["REQ_QTY"].ToString());
                yTBM.Add(r["TBM"].ToString());
                yCUR.Add(r["CURE"].ToString());
            }

        }
    }
}
