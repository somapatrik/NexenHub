using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexenHub.Class;
using NexenHub.Models;

namespace NexenHub.Pages
{
    public class eslModel : PageModel
    {

        public string GeneratedLayout { get; set; }

        public Esl esldata;

        public void OnGet(string cartid)
        {
            if (cartid.Length == 5)
            {
                esldata = new Esl(cartid);
                if (esldata.VALID)
                    GeneratedLayout = ReplaceLabels(GetLayout());
            }

            DBOra db = new DBOra("select * from TB_IN_H_PROD_ESL_LAYOUT");
            DataTable dt = db.ExecTable();
            string nevim = dt.Rows[0]["LAYOUT"].ToString();

        }

        private string ReplaceLabels(string layout)
        {
            layout = layout.Contains("{{LOT_ID}}") ? layout.Replace("{{LOT_ID}}", esldata.LOT_ID) : layout;
            layout = layout.Contains("{{CART_ID}}") ? layout.Replace("{{CART_ID}}", esldata.CART_ID) : layout;
            layout = layout.Contains("{{ITEM_NAME}}") ? layout.Replace("{{ITEM_NAME}}", esldata.ITEM_NAME) : layout;
            layout = layout.Contains("{{ITEM_ID}}") ? layout.Replace("{{ITEM_ID}}", esldata.ITEM_ID) : layout;
            layout = layout.Contains("{{PROD_TIME}}") ? layout.Replace("{{PROD_TIME}}", esldata.PROD_TIME) : layout;
            return layout;
        }


        private string GetLayout()
        {
            return "<div class=\"container-fluid h-auto border text-center\" style=\"font-size:larger\"> "
                    + "        <div class=\"row\"> "
                    + "            <div class=\"col-8\"> "
                    + "                <div class=\"row\"> "
                    + "                    <div class=\"col\">{{Barcode LOT_ID}}</div> "
                    + "                </div> "
                    + "                <div class=\"row\"> "
                    + "                    <div class=\"col\">{{LOT_ID}}</div> "
                    + "                </div> "
                    + "            </div> "
                    + "            <div class=\"col-4 bg-dark\"> "
                    + "                <span class=\"text-light\" style=\"font-size:xx-large\">{{CART_ID}}</span> "
                    + "            </div> "
                    + "        </div> "
                    + "        <div class=\"row\"> "
                    + "            <div class=\"col font-weight-bold\"> "
                    + "                <span style=\"font-size:xx-large;\">{{ITEM_NAME}}</span> "
                    + "            </div> "
                    + "        </div> "
                    + "        <div class=\"row\"> "
                    + "            <div class=\"col font-weight-bold\"> "
                    + "                <span style=\"font-size:x-large;\">{{COMPOUND}}</span> "
                    + "            </div> "
                    + "        </div> "
                    + "        <div class=\"row\"> "
                    + "            <div class=\"col border font-weight-bold\"><span style=\"font-size:xx-large;\">{{ITEM_ID}}</span></div> "
                    + "            <div class=\"col border font-weight-bold\"><span style=\"font-size:xx-large;\">{{QTY}}</span></div> "
                    + "            <div class=\"col border font-weight-bold\"><span style=\"font-size:xx-large;\">{{PROD_TYPE}}</span></div> "
                    + "        </div> "
                    + "        <div class=\"row\"> "
                    + "            <div class=\"col border\"> "
                    + "                <span style=\"font-size:xx-large\">{{PROD_TIME}}</span> "
                    + "            </div> "
                    + "            <div class=\"col\"> "
                    + "                <div class=\"row\"> "
                    + "                    <div class=\"col border\">{{TIME1}}</div> "
                    + "                </div> "
                    + "                <div class=\"row\"> "
                    + "                    <div class=\"col border\">{{TIME2}}</div> "
                    + "                </div> "
                    + "            </div> "
                    + "        </div> "
                    + "        <div class=\"row\"> "
                    + "            <div class=\"col\"></div> "
                    + "            <div class=\"col border font-weight-bold\"><span style=\"font-size:xx-large;\">{{Size?}}</span></div> "
                    + "        </div> "
                    + "    </div> ";
        }
    }
}
