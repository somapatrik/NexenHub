using NexenHub.Class;
using System.Data;
using System.Reflection.Metadata;

namespace NexenHub.Models
{
    public class Tire
    {
        public string Barcode { get; set; }
        public LotItem GtLot { get; set; }
        public LotItem TireLot { get; set; }

        public Item GtItem { get; set; }
        public Item TireItem { get; set; }

        public bool IsGt { get; set; }
        public bool IsTire { get; set; }

        private GlobalDatabase dbglob = new GlobalDatabase();

        public Tire(string code)
        {

            if (code.Length == 10 || code.Length == 15)
            {
                DataTable dt = dbglob.BarcodeToLotId(code);
                if (dt.Rows.Count > 0)
                {
                    Barcode = dt.Rows[0]["BARCODE"].ToString();
                    string tbmLot = dt.Rows[0]["TBM_LOT_ID"].ToString();
                    string cureLot = dt.Rows[0]["CURE_LOT_ID"].ToString();

                    if (!string.IsNullOrEmpty(tbmLot))
                    {
                        IsGt = true;
                        GtLot = new LotItem(tbmLot);
                        GtLot.LoadHistory();
                        GtLot.RemoveUselessHistory();

                        GtItem = new Item(GtLot.ID);
                    }

                    if (!string.IsNullOrEmpty(cureLot))
                    {
                        IsTire = true;
                        TireLot = new LotItem(cureLot);
                        TireLot.LoadHistory();
                        TireLot.RemoveUselessHistory();

                        TireItem = new Item(TireLot.ID);
                    }

                }
            }
        }

    }
}
