using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexenHub.Class;

namespace NexenHub.Pages
{
    [Authorize(Roles = "Operator")]
    public class _3dchartModel : PageModel
    {

        public List<dataVis> dataList = new List<dataVis>();

        public void OnGet()
        {
            DateTime prodNow = DateTime.Now.AddHours(-6);

            StringBuilder q = new StringBuilder();
            q.AppendLine("SELECT /*+INDEX(PROD IX_PR_M_PROD_5)*/");
            q.AppendLine("EQ.EQ_NAME,");
            q.AppendLine("CASE WHEN EQ.EQ_ID < '10039' THEN '5' ELSE '0' END X,");
            q.AppendLine("CASE");
            q.AppendLine("WHEN(EQ.EQ_ID - 10033) <= 5 THEN ((EQ.EQ_ID - 10033)*2)");
            q.AppendLine("ELSE ((EQ.EQ_ID - 10039)*2)");
            q.AppendLine("END Y,");
            q.AppendLine("SUM(PROD.PROD_QTY) Z");
            q.AppendLine("FROM TB_PR_M_PROD PROD");
            q.AppendLine("LEFT JOIN TB_EQ_M_EQUIP EQ on EQ.EQ_ID = PROD.EQ_ID");
            q.AppendLine("WHERE PROD_DATE = '"+ prodNow.ToString("yyyyMMdd") +"'");
            q.AppendLine("AND PROD.WC_ID = 'T'");
            q.AppendLine("AND PROD.PLANT_ID = 'P500'");
            q.AppendLine("AND PROD.USE_YN = 'Y'");
            q.AppendLine("AND SHIFT IS NOT NULL");
            q.AppendLine("GROUP BY EQ.EQ_NAME, EQ.EQ_ID");
            q.AppendLine("ORDER BY EQ_NAME");

            DBOra db = new DBOra(q.ToString());
            foreach (DataRow r in db.ExecTable().Rows)
                dataList.Add(new dataVis() { X = r["X"].ToString(), Y = r["Y"].ToString(), Z = r["Z"].ToString(), Name=r["EQ_NAME"].ToString()});
        }

    }

    public class dataVis
    {
        public string X { get; set; }
        public string Y { get; set; }
        public string Z { get; set; }
        public string Name { get; set; }
    }

}
