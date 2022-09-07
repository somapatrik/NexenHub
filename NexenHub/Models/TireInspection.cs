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
        #region JSON properties

        public string Barcode { get; set; }
        public ProductionInfo GtProduction { get; set; }
        public ProductionInfo TireProduction { get; set; }
        public List<FertInspectionResult> TireInspectionResult { get; set; }
        public EMR TireEMR { get; set; }
        public List<UsedHALB> TireUsedHALB { get; set; }

        #endregion

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

                        FillInspectionResult(Barcode);

                        FillUsedHALB(Barcode);
                    }

                    if (!string.IsNullOrEmpty(tbmLot))
                    {
                        LotItem TbmLot = new LotItem(tbmLot);
                        TbmLot.LoadHistory();
                        TbmLot.RemoveUselessHistory();

                        GtProduction = FillProductionInfo(TbmLot);

                        if (!string.IsNullOrEmpty(TbmLot.PROTOTYPE_ID))
                            TireEMR = new EMR(TbmLot.PROTOTYPE_ID);
                    }

                }

            }

        }

        private void FillUsedHALB(string barcode)
        {
            DataTable dt = dbglob.GetUsedHalb(barcode);

            TireUsedHALB = new List<UsedHALB>();

            foreach (DataRow row in dt.Rows)
            {
                UsedHALB used = new UsedHALB() 
                {
                    ITEM_ID = row["ITEM_ID"].ToString(),
                    ITEM_NAME = row["ITEM_NAME"].ToString(),
                    CART_ID = row["CART_ID"].ToString(),
                    INPUT_LOT_ID = row["INPUT_LOT_ID"].ToString(),
                    EQ_NAME = row["EQ_NAME"].ToString(),
                    EQ_ID = row["EQ_ID"].ToString(),
                    PROD_DATE = DateTime.Parse(row["PROD_DATE"].ToString()),
                    INPUT_TIME = DateTime.Parse(row["INPUT_TIME"].ToString()),
                    AGING_T = row["AGING_T"].ToString(),
                    IO_POSID = row["IO_POSID"].ToString(),
                    SHIFT = row["SHIFT"].ToString(),
                    MEMBER_NAME = row["MEMBER_NAME"].ToString(),
                    CP_LOT_ID = row["CP_LOT_ID"].ToString(),
                    CP_CART_ID = row["CP_CART_ID"].ToString(),
                    LOT_CHECK = row["LOT_CHECK"].ToString() == "True",

                };

                TireUsedHALB.Add(used);

            }
        }

        private void FillInspectionResult(string lot)
        {
            TireInspectionResult = new List<FertInspectionResult>();

            DataTable dt = dbglob.GetFertInspectionResult(lot);

            foreach (DataRow row in dt.Rows)
            {
                FertInspectionResult res = new FertInspectionResult();

                res.SEQ = int.Parse(row["INSP_SEQ"].ToString());
                res.PROC = row["PROC_ID"].ToString();
                res.InspectionTime = DateTime.Parse(row["INSPECTION_TIME"].ToString());
                res.SHIFT = row["SHIFT"].ToString();
                res.SHIFT_RBG = row["SHIFT_RGB"].ToString();
                res.BAD_ID = row["BAD_ID"].ToString();
                res.BAD_GRADE = row["BAD_GRADE"].ToString();
                res.LOC_MOLD = row["LOC_MOLD"].ToString();
                res.LOC_SIDE = row["LOC_SIDE"].ToString();
                res.LOC_ZONE = row["LOC_ZONE"].ToString();
                res.LOC_POSITION = row["LOC_POSITION"].ToString();
                res.CQ2 = row["CQ2"].ToString();
                res.UserName = row["ENT_USER"].ToString();

                TireInspectionResult.Add(res);
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

        #region Models

        public class UsedHALB
        {
            public string ITEM_ID { get; set; }
            public string ITEM_NAME { get; set; }
            public string CART_ID { get; set; }
            public string INPUT_LOT_ID { get; set; }
            public string EQ_NAME { get; set; }
            public string EQ_ID { get; set; }
            public DateTime PROD_DATE { get; set; }
            public DateTime INPUT_TIME { get; set; }
            public string AGING_T { get; set; }
            public string IO_POSID { get; set; }
            public string SHIFT { get; set; }
            public string MEMBER_NAME { get; set; }
            public string CP_LOT_ID { get; set; }
            public string CP_CART_ID { get; set; }
            public bool LOT_CHECK { get; set; }
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

        #endregion

    }
}
