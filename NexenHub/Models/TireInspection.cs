using NexenHub.Class;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Models
{
    public class TireInspection
    {

        public string Barcode { get; set; }
        public LotItem TbmLot { get; set; }
        public LotItem CureLot { get; set; }

        private GlobalDatabase dbglob = new GlobalDatabase();

        public TireInspection(string CodeId)
        {
            // Can load barcode or LOT_ID
            if (CodeId.Length == 10 || CodeId.Length == 15)
            {
                DataTable dt = dbglob.BarcodeToLotId(CodeId);
            }

        }



    }
}
