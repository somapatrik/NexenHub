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
        public FertInspectionResult TireInspectionResult { get; set; }


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

                        TireInspectionResult = FillInspectionResult(Barcode);
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

        private FertInspectionResult FillInspectionResult(string lot)
        {
            DataTable dt = dbglob.GetFertInspectionResult(lot);
            FertInspectionResult res = new FertInspectionResult();
            if (dt.Rows.Count > 0)
            {
                res.SEQ = int.Parse(dt.Rows[0]["INSP_SEQ"].ToString());
                res.PROC = dt.Rows[0]["PROC_ID"].ToString();
                res.InspectionTime = DateTime.Parse(dt.Rows[0]["INSPECTION_TIME"].ToString());
                res.SHIFT = dt.Rows[0]["SHIFT"].ToString();
                res.SHIFT_RBG = dt.Rows[0]["SHIFT_RGB"].ToString();
                res.BAD_ID = dt.Rows[0]["BAD_ID"].ToString();
                res.BAD_GRADE = dt.Rows[0]["BAD_GRADE"].ToString();
                res.LOC_MOLD = dt.Rows[0]["LOC_MOLD"].ToString();
                res.LOC_SIDE = dt.Rows[0]["LOC_SIDE"].ToString();
                res.LOC_ZONE = dt.Rows[0]["LOC_ZONE"].ToString();
                res.LOC_POSITION = dt.Rows[0]["LOC_POSITION"].ToString();
                res.CQ2 = dt.Rows[0]["CQ2"].ToString();
                res.UserName = dt.Rows[0]["ENT_USER"].ToString();
            }
            return res;
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
                // ITEM_DETAIL = lotItem.WC_ID == "U" ? FillItemInfo(new Item(lotItem.ID)) : null,
                ITEM_DETAIL = FillItemInfo(new Item(lotItem.ID)),
                EQ_ID = lotItem.EQ_ID,
                PROD_TYPE = lotItem.ProdType,
                PROD_TYPE_DESC = lotItem.ProdTypeDesc,
                EQ_NAME = lotItem.EQ_NAME,
                USER_NAME = lotItem.USER_NAME,
                USER_ID = lotItem.USER_ID,
                LOT_ID = lotItem.LOT_ID
                
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
            public string LOT_ID { get; set; }
            public string EQ_ID { get; set; }
            public string EQ_NAME { get; set; }
            public string PROD_TYPE { get; set; }
            public string PROD_TYPE_DESC { get; set; }
            public DateTime PRODUCTION_DATE { get; set; }
            public string ITEM_ID { get; set; }
            public string ITEM_NAME { get; set; }
            public bool TEST { get; set; }
            public string USER_ID { get; set; }
            public string USER_NAME { get; set; }
            public ItemInfo ITEM_DETAIL { get; set; }
            public List<LotHisItem> LOT_HISTORY { get; set; }

        }

        public class FertInspectionResult
        {
            public int SEQ { get; set; }
            public string PROC { get; set; }
            public DateTime InspectionTime { get; set; }
            public string SHIFT { get; set; }
            public string SHIFT_RBG { get; set; }
            public string BAD_ID { get; set; }
            public string BAD_GRADE { get; set; }
            public string LOC_MOLD { get; set; }
            public string LOC_SIDE { get; set; }
            public string LOC_ZONE { get; set; }
            public string LOC_POSITION { get; set; }
            public string CQ2 { get; set; }
            public string UserName { get; set; }
        }

    }
}
