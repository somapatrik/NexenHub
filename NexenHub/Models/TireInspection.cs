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
        public ProductionInfo GtProduction { get; set; }
        public ProductionInfo TireProduction { get; set; }

        private GlobalDatabase dbglob = new GlobalDatabase();
        public TireInspection(string CodeId)
        {
            // Can load barcode or LOT_ID
            if (CodeId.Length == 10 || CodeId.Length == 15)
            {
                DataTable dt = dbglob.BarcodeToLotId(CodeId);
                if (dt.Rows.Count > 0)
                {
                    Barcode = dt.Rows[0]["BARCODE"].ToString();
                    string tbmLot = dt.Rows[0]["TBM_LOT_ID"].ToString();
                    string cureLot = dt.Rows[0]["CURE_LOT_ID"].ToString();

                    if (!string.IsNullOrEmpty(cureLot)) 
                    { 
                        LotItem CureLot = new LotItem(cureLot);
                        CureLot.LoadHistory();
                        CureLot.RemoveUselessHistory();
   
                        TireProduction = FillProductionInfo(CureLot);
                    }

                    if (!string.IsNullOrEmpty(tbmLot)) 
                    { 
                        LotItem TbmLot = new LotItem(tbmLot);
                        TbmLot.LoadHistory();
                        TbmLot.RemoveUselessHistory();

                        GtProduction = FillProductionInfo(TbmLot);
                    }

                }

            }

        }

        private ProductionInfo FillProductionInfo(LotItem lotItem)
        {
            ProductionInfo productionInfo = new ProductionInfo()
            {
                ITEM_ID = lotItem.ID,
                ITEM_NAME = lotItem.Name,
                PRODUCTION_DATE = lotItem.dateLotTime,
                LOT_HISTORY = lotItem.History,
                TEST = lotItem.Test,
                ITEM_DETAIL = lotItem.WC_ID == "U" ? FillItemInfo(new Item(lotItem.ID)) : null,
                EQ_ID = lotItem.EQ_ID
                
            };
            return productionInfo;
        }

        private ItemInfo FillItemInfo(Item selectedItem)
        {
            ItemInfo itemdetail = new ItemInfo()
            {
                ITEM_ID = selectedItem.ID,
                ITEM_NAME = selectedItem.ITEM_NAME,
                PATTERN = selectedItem.PATTERN,
                SECTION_WIDTH = selectedItem.SECTION_WIDTH,
                SERIES = selectedItem.SERIES,
                INCH = selectedItem.INCH,
                OE = selectedItem.OE
            };

            return itemdetail;
        }


        public class ItemInfo
        {
            public string ITEM_ID { get; set; }
            public string ITEM_NAME { get; set; }
            public string PATTERN { get; set; }
            public string SECTION_WIDTH { get; set; }
            public string SERIES { get; set; }
            public string INCH { get; set; }
            public bool OE { get; set; }

        }

        public class ProductionInfo
        {
            public string EQ_ID { get; set; }
            public string EQ_NAME { get; set; }
            public DateTime PRODUCTION_DATE { get; set; }
            public string ITEM_ID { get; set; }
            public string ITEM_NAME { get; set; }
            public bool TEST { get; set; }
           // public string MEMBER_ID { get; set; }
            public string USER_NAME { get; set; }
            public ItemInfo ITEM_DETAIL { get; set; }
            public List<LotHisItem> LOT_HISTORY { get; set; }

        }


    }
}
