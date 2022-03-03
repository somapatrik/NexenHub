using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexenHub.Class;

namespace NexenHub.Pages
{
    public class VisCurModel : PageModel
    {
        public List<dataVis> dataList = new List<dataVis>();
        public void OnGet()
        {
            DateTime prodNow = DateTime.Now.AddHours(-6);

            StringBuilder q = new StringBuilder();
            q.AppendLine("SELECT ");
            q.AppendLine("EQ.EQ_NAME,");
            q.AppendLine("CASE");
            q.AppendLine("WHEN EQ.PROC_ID = 'UA' THEN 24");
            q.AppendLine("WHEN EQ.PROC_ID = 'UB' THEN 16");
            q.AppendLine("WHEN EQ.PROC_ID = 'UC' THEN 8");
            q.AppendLine("ELSE 0");
            q.AppendLine("END X,");
            q.AppendLine("CASE");
            q.AppendLine("WHEN EQ.PROC_ID = 'UA' THEN((EQ.EQ_ID - 10051)*1)");
            q.AppendLine("WHEN EQ.PROC_ID = 'UB' THEN((EQ.EQ_ID - 10072)*1)");
            q.AppendLine("WHEN EQ.PROC_ID = 'UC' THEN((EQ.EQ_ID - 10094)*1)");
            q.AppendLine("ELSE((EQ.EQ_ID - 10115)*1)");
            q.AppendLine("END Y,");
            q.AppendLine("NVL(A.PRODQTY, 0) Z");
            q.AppendLine("FROM TB_EQ_M_EQUIP EQ");
            q.AppendLine("LEFT JOIN(SELECT /*+INDEX(PROD IX_PR_M_PROD_5)*/");
            q.AppendLine("                    PROD.EQ_ID, SUM(PROD.PROD_QTY) PRODQTY");
            q.AppendLine("                    FROM TB_PR_M_PROD PROD");
            q.AppendLine("                    WHERE PROD_DATE = '"+prodNow.ToString("yyyyMMdd")+"'");
            q.AppendLine("                    AND PROD.WC_ID = 'U'");
            q.AppendLine("                    AND PROD.PLANT_ID = 'P500'");
            q.AppendLine("                    AND PROD.USE_YN = 'Y'");
            q.AppendLine("                    AND SHIFT IS NOT NULL");
            q.AppendLine("                    GROUP BY PROD.EQ_ID) A on A.EQ_ID = EQ.EQ_ID");
            q.AppendLine("where EQ.WC_ID = 'U'");
            q.AppendLine("AND EQ.USE_YN = 'Y'");
            q.AppendLine("AND EQ.EQ_TYPE = 'P'");
            q.AppendLine("ORDER BY EQ.EQ_ID");

            DBOra db = new DBOra(q.ToString());
            foreach (DataRow r in db.ExecTable().Rows)
                dataList.Add(new dataVis() { X = r["X"].ToString(), Y = r["Y"].ToString(), Z = r["Z"].ToString(), Name = r["EQ_NAME"].ToString() });
        }

        public class dataVis
        {
            public string X { get; set; }
            public string Y { get; set; }
            public string Z { get; set; }
            public string Name { get; set; }
        }
    }
}
