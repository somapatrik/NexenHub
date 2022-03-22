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

        private string _LOT_ID;

        public string LOT_ID
        {
            get { return _LOT_ID; }
            set
            {
                _LOT_ID = value;
                InitValues();
                LoadFromDb();
            }
        }

        public DateTime dateExpiry
        {
            get
            {
                DateTime outdate = DateTime.MinValue;
                DateTime.TryParseExact(ExpiryDate,"yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out outdate );
                return outdate;
            }
        }

        public DateTime dateEvent
        {
            get
            {
                DateTime outdate = DateTime.MinValue;
                DateTime.TryParseExact(EventTime, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out outdate);
                return outdate;
            }
        }

        public string LotID;
        public string ID;
        public string Name;
        public string State;
        public string CartState;
        public double StockQty;
        public string LotState;
        public string Grp;
        public string Kind;
        public string ProcId;
        public string ExpiryDate;
        public string EventTime;
        public bool ExpiryDateResult;
        public string Division;
        public bool Valid;

        public List<LotHisItem> History;

        public LotItem(string LOTID)
        {
            _LOT_ID = LOTID;
            InitValues();
            LoadFromDb();
        }

        private void InitValues()
        {
            LotID = "";
            ID = "";
            Name = "";
            State = "";
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
            Valid = false;

            History = new List<LotHisItem>();
        }

        private void LoadFromDb()
        {
            if (!string.IsNullOrEmpty(_LOT_ID))
            {
                DataTable dt = dbglob.GetLotInfo(_LOT_ID);
                if (dt.Rows.Count > 0)
                {
                    LotID = dt.Rows[0]["INPUT_LOT_ID"].ToString();
                    ID = dt.Rows[0]["INPUT_ITEM_ID"].ToString();
                    Name = dt.Rows[0]["INPUT_ITEM_NAME"].ToString();
                    State = dt.Rows[0]["INPUT_ITEM_STATE"].ToString();
                    CartState = dt.Rows[0]["INPUT_ITEM_CART_STATE"].ToString();
                    StockQty = double.Parse(dt.Rows[0]["INPUT_ITEM_STOCK_QTY"].ToString());
                    LotState = dt.Rows[0]["INPUT_ITEM_LOT_STATE"].ToString();
                    Grp = dt.Rows[0]["INPUT_ITEM_GRP"].ToString();
                    Kind = dt.Rows[0]["INPUT_ITEM_KIND"].ToString();
                    ProcId = dt.Rows[0]["INPUT_ITEM_PROC_ID"].ToString();
                    ExpiryDate = dt.Rows[0]["INPUT_ITEM_EXPIRY_DATE"].ToString();
                    EventTime = dt.Rows[0]["INPUT_ITEM_EVENT_TIME"].ToString();
                    ExpiryDateResult = dt.Rows[0]["INPUT_ITEM_EXPIRY_DATE_RESULT"].ToString() == "1" ? true : false;
                    Division = dt.Rows[0]["INPUT_ITEM_DEVISION"].ToString();
                    Valid = true;
                }

                foreach (DataRow row in dbglob.GetLotHis(_LOT_ID).Rows)
                {
                    History.Add(new LotHisItem()
                    {
                        transDate = DateTime.Parse(row["TRANDATE"].ToString()),
                        locationName = row["LOCATION"].ToString(),
                        itemState = row["ITEMSTATE"].ToString(),
                        lotState = row["LOTSTATE"].ToString(),
                        qtyUnit = row["QTY"].ToString()
                    }) ;
                    
                }

            }
        }


    }
}
