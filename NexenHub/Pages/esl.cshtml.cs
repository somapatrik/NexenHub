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

        public Esl EslModel;
        public GlobalDatabase globalDatabase = new GlobalDatabase();

        public void OnGet(string cartid)
        {
            if (cartid.Length == 5)
            {
                cartid = cartid.ToUpper();
                EslModel = new Esl(cartid);
                if (EslModel.VALID)
                    GeneratedLayout = ReplaceLabels(GetLayout(cartid));
            }
        }

        private string ReplaceLabels(string layout)
        {
            layout = layout.Contains("{{LOT_ID}}") ? layout.Replace("{{LOT_ID}}", EslModel.LOT_ID) : layout;
            layout = layout.Contains("{{CART_ID}}") ? layout.Replace("{{CART_ID}}", EslModel.CART_ID) : layout;
            layout = layout.Contains("{{ITEM_NAME}}") ? layout.Replace("{{ITEM_NAME}}", EslModel.ITEM_NAME) : layout;
            layout = layout.Contains("{{ITEM_ID}}") ? layout.Replace("{{ITEM_ID}}", EslModel.ITEM_ID) : layout;
            layout = layout.Contains("{{PROD_TIME}}") ? layout.Replace("{{PROD_TIME}}", EslModel.PROD_TIME) : layout;
            return layout;
        }

        private string GetLayout(string cartid)
        {
            DataTable dt = globalDatabase.SP_IN_H_PROD_LAYOUT(cartid);
            return dt.Rows[0]["LAYOUT"].ToString();
        }
    }
}
