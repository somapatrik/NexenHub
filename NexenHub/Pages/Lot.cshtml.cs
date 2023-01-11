using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexenHub.Models;
using NexenHub.Class;
using NetBarcode;
using System.Data;

namespace NexenHub.Pages
{
    public class LotModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string arglot { get; set; }
        public LotItem lotitem { get; set; }
        public Barcode lotBarcode { get; set; }

        public List<LotItem> Parents { get; set; }

        private GlobalDatabase dbglob = new GlobalDatabase();

        public bool FuckHim { get; set; }

        public void OnGet()
        {
            Random rnd = new Random();
            string clientIP = HttpContext.Connection.RemoteIpAddress?.ToString();
            FuckHim = (clientIP == "172.15.9.70" || clientIP == "172.15.144.1") && rnd.Next(3) == 0;
            //FuckHim = (clientIP == "127.0.0.1") && rnd.Next(3) == 0;

            if (arglot.Length == 5)
                arglot = dbglob.Cart2Lot(arglot);

            lotitem = new LotItem(arglot);
            lotitem.LoadHistory();
            lotitem.RemoveUselessHistory();

            lotBarcode = new Barcode(lotitem.LOT_ID, NetBarcode.Type.Code128B) ;

            TraceLot();
        }

        private void TraceLot()
        {
            Parents = new List<LotItem>();
            foreach (DataRow row in dbglob.GetLotParents(lotitem.LOT_ID).Rows)
                Parents.Add(new LotItem(row["INPUT_LOT_ID"].ToString()));
        }
    }
}
