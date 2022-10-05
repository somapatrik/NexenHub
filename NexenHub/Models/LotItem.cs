using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NexenHub.Class;

namespace NexenHub.Models
{
    public class LotItem
    {

        GlobalDatabase dbglob = new GlobalDatabase();

        #region Properties

        private string _LOT_ID;
        public string LOT_ID
        {
            get { return _LOT_ID; }
            set
            {
                _LOT_ID = value;
               // InitValues();
                LoadFromDb();
            }
        }

        public DateTime dateLotTime
        {
            get => ConvertDate(LotTime);
        }

        public DateTime dateExpiry
        {
            get => ConvertDate(ExpiryDate);
        }

        public DateTime dateEvent
        {
            get => ConvertDate(EventTime);
        }

        public DateTime dateAgingTime
        {
            get => ConvertDate(AgingTime);
        }

        public DateTime dateProdTime
        {
            get => ConvertDate(ProdTime);
        }

        public string LotID { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public string EQ_ID { get; set; }
        public string EQ_NAME { get; set; }
        public string CART_ID { get; set; }
        public string CartState { get; set; }
        public double StockQty { get; set; }
        public string LotState { get; set; }
        public string Grp { get; set; }
        public string Kind { get; set; }
        public string WC_ID { get; set; }
        public string ProcId { get; set; }
        public string ProdType { get; set; }
        public string ProdTypeDesc { get; set; }
        public string ProdTime { get; set; }
        public string AgingTime { get; set; }
        public string LotTime { get; set; }
        public string ExpiryDate { get; set; }
        public string EventTime { get; set; }
        public bool ExpiryDateResult { get; set; }
        public string Division { get; set; }
        public bool Test { get; set; }
        public string PROTOTYPE_ID { get; set; }
        public string PROTOTYPE_VER { get; set; }
        public string USER_ID { get; set; }
        public string USER_NAME { get; set; }
        public string TREAD_WIDTH { get; set; }
        public string COMPOUND { get; set; }

        public bool Valid { get; set; }
        public List<LotHisItem> History { get; set; }

        #endregion

        public LotItem(string LOTID)
        {
            _LOT_ID = LOTID;

            // InitValues();
             History = new List<LotHisItem>();

            LoadFromDb();

            if (ProcId == "E1" || ProcId == "E2") 
            { 
                LoadTreadWidth();
                LoadExtCompound();
            }
        }

        private void InitValues()
        {
            LotID = "";
            ID = "";
            Name = "";
            State = "";
            CART_ID = "";
            CartState = "";
            StockQty = 0;
            LotState = "";
            Grp = "";
            Kind = "";
            ProcId = "";
            ExpiryDate = "";
            EventTime = "";
            ExpiryDateResult = false;
            Division = "";
            ProdTime = "";
            AgingTime = "";
            Test = false;
            WC_ID = "";
            EQ_ID = "";
            EQ_NAME = "";
            ProdType = "";
            ProdTypeDesc = "";

            Valid = false;

            History = new List<LotHisItem>();
        }

        private void LoadFromDb()
        {
            Valid = false;
            if (!string.IsNullOrEmpty(_LOT_ID))
            {
                DataTable dt = dbglob.GetLotInfo(_LOT_ID);
                if (dt.Rows.Count > 0)
                {
                    LotID = dt.Rows[0]["LOT_ID"].ToString();
                    ID = dt.Rows[0]["ITEM_ID"].ToString();
                    Name = dt.Rows[0]["ITEM_NAME"].ToString();
                    State = dt.Rows[0]["ITEM_STATE"].ToString();
                    CART_ID = dt.Rows[0]["CART_ID"].ToString();
                    CartState = dt.Rows[0]["CART_STATE"].ToString();
                    StockQty = double.Parse(dt.Rows[0]["STOCK_QTY"].ToString());
                    LotState = dt.Rows[0]["LOT_STATE"].ToString();
                    Grp = dt.Rows[0]["ITEM_GRP"].ToString();
                    Kind = dt.Rows[0]["ITEM_KIND"].ToString();
                    ProcId = dt.Rows[0]["ITEM_PROC_ID"].ToString();
                    LotTime = dt.Rows[0]["LOT_TIME"].ToString();

                    ProdTime = dt.Rows[0]["PROD_TIME"].ToString();
                    AgingTime = dt.Rows[0]["AGING_TIME"].ToString();

                    ExpiryDate = dt.Rows[0]["EXPIRY_DATE"].ToString();
                    EventTime = dt.Rows[0]["EVENT_TIME"].ToString();
                    ExpiryDateResult = dt.Rows[0]["EXPIRY_DATE_RESULT"].ToString() == "1" ? true : false;
                    Division = dt.Rows[0]["DEVISION"].ToString();
                    Test = dt.Rows[0]["LOT_TEST_YN"].ToString() == "Y";
                    WC_ID = dt.Rows[0]["PROD_WC_ID"].ToString();
                    EQ_ID = dt.Rows[0]["EQ_ID"].ToString();
                    EQ_NAME = dt.Rows[0]["EQ_NAME"].ToString();
                    ProdType = dt.Rows[0]["PROD_TYPE"].ToString();
                    ProdTypeDesc = dt.Rows[0]["PROD_TYPE_DESC"].ToString();

                    PROTOTYPE_ID = dt.Rows[0]["PROTOTYPE_ID"].ToString();
                    PROTOTYPE_VER = dt.Rows[0]["PROTOTYPE_VER"].ToString();

                    USER_ID = dt.Rows[0]["USER_ID"].ToString();
                    USER_NAME = dt.Rows[0]["USER_NAME"].ToString();

                    //COMPOUND = NullIfEmpty(dt.Rows[0]["COMPOUND_NAME"].ToString());

                    Valid = true;
                }

            }
        }

        #region History

        public void LoadHistory()
        {
            if (!string.IsNullOrEmpty(_LOT_ID))
            {
                foreach (DataRow row in dbglob.GetLotHis(_LOT_ID).Rows)
                {
                    History.Add(new LotHisItem()
                    {
                        transDate = DateTime.Parse(row["TRANDATE"].ToString()),
                        locationName = row["LOCATION"].ToString(),
                        itemState = row["ITEMSTATE"].ToString(),
                        lotState = row["LOTSTATE"].ToString(),
                        qtyUnit = row["QTY"].ToString()
                    });

                }
            }
        }

        public void LoadHistoryClean()
        {
            if (!string.IsNullOrEmpty(_LOT_ID))
            {
                foreach (DataRow row in dbglob.GetLotHisClean(_LOT_ID).Rows)
                {
                    History.Add(new LotHisItem()
                    {
                        transDate = DateTime.Parse(row["TRANDATE"].ToString()),
                        locationName = row["LOCATION"].ToString(),
                        itemState = row["ITEMSTATE"].ToString(),
                        lotState = row["LOTSTATE"].ToString(),
                        qtyUnit = row["QTY"].ToString()
                    });

                }
            }
        }

        public void RemoveUselessHistory()
        {
            if (History.Count > 0)
            {
                List<LotHisItem> temp = new List<LotHisItem>();
                History.ForEach(x=>temp.Add(x));
                History.Clear();

                foreach (LotHisItem his in temp)
                {
                    // Don´t care about repeating history. HISTORY SHOULDN´T REPEAT ITSELF
                    if (History.Find(x => x.itemState == his.itemState &&
                                         x.locationName == his.locationName &&
                                         x.lotState == his.lotState &&
                                         x.qtyUnit == his.qtyUnit) == null)
                    {
                        History.Add(his);
                    }
                }

            }
        }

        #endregion

        public void LoadTreadWidth()
        {
            TREAD_WIDTH = NullIfEmpty(dbglob.GetTreadWidth(LOT_ID));
        }

        public void LoadExtCompound()
        {
            COMPOUND = NullIfEmpty(dbglob.GetExtCompound(LOT_ID));
        }

        #region Utility

        private string NullIfEmpty(string str)
        {
            return string.IsNullOrEmpty(str) ? null : str;
        }

        private DateTime ConvertDate(string time)
        {
            DateTime outdate = DateTime.MinValue;
            DateTime.TryParseExact(time, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out outdate);
            return outdate;
        }

        #endregion
    }
}
