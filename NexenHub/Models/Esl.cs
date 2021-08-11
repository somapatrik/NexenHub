using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using NetBarcode;
using NexenHub.Class;
using Oracle.ManagedDataAccess.Client;

namespace NexenHub.Models
{
    public class Esl
    {
        private GlobalDatabase _Database = new GlobalDatabase();

        private string _CART_ID;

        private string _LOT_ID;

        private string _LAYOUT;

        private List<String> _COMMON_ITEM_ID = new List<string>();

        #region Public properties used for API

        public string CART_ID 
        {
            get { return _CART_ID; } 
            set 
            { 
                _CART_ID = value.ToUpper();
                LoadFromDb();
            }
        }
        public string LOT_ID 
        { 
            get { return _LOT_ID; } 
            set
            {
                _LOT_ID = value.ToUpper();
                LoadFromDb();
            } 
        }
        public string LOT_ID_BARCODE_128 { get; set; }
        public string EVENT_TIME { get; set; }
        public string ITEM_ID { get; set; }
        public string WC_ID { get; set; }
        public string PROC_ID { get; set; }
        public string ITEM_STATE { get; set; }
        public string PROD_TIME { get; set; }
        public string USER_NAME { get; set; }
        public string WO_NO { get; set; }
        public string PROD_TYPE { get; set; }
        public string PROD_TYPE_NAME { get; set; }
        public string PROD_QTY { get; set; }
        public string BAL_YN { get; set; }
        public string TEST_YN { get; set; }
        public string SAVE_TYPE { get; set; }
        public string ENT_DT { get; set; }
        public string ENT_USER_ID { get; set; }
        public string ITEM_NAME { get; set; }
        public string CART_STATE { get; set; }
        public string EXPIRY_DATE { get; set; }
        public string AGING_TIME { get; set; }
        public string TREAD_WIDTH { get; set; }
        public string COMPOUND { get; set; }
        public string IO_POSID { get; set; }
        public string EQ_NAME { get; set; }
        public string RCV_DT { get; set; }
        public string TEMP01 { get; set; }
        public List<string> COMMON_ITEM_ID
        {
            get
            {
                return _COMMON_ITEM_ID;
            }

            set
            {
                _COMMON_ITEM_ID = value;
            }
        }

        public bool VALID { get; set; }

        #endregion

        #region Constructors

        public Esl()
        { 

        }

        #endregion

        private void LoadFromDb()
        {
            VALID = false;
            if (CheckInput())
            {
                DataTable dt = _Database.SP_DC_H_PROD_API_ESL(_CART_ID,_LOT_ID);

                if (dt.Rows.Count > 0)
                {
                    VALID = true;
                    
                    if (!string.IsNullOrEmpty(_CART_ID))
                        _LOT_ID = dt.Rows[0]["LOT_ID"].ToString();
                    else
                        _CART_ID = dt.Rows[0]["CART_ID"].ToString();

                    EVENT_TIME = dt.Rows[0]["EVENT_TIME"].ToString();
                    ITEM_ID = dt.Rows[0]["ITEM_ID"].ToString();
                    WC_ID = dt.Rows[0]["WC_ID"].ToString();
                    PROC_ID = dt.Rows[0]["PROC_ID"].ToString();
                    ITEM_STATE = dt.Rows[0]["ITEM_STATE"].ToString();
                    CART_STATE = dt.Rows[0]["CART_STATE"].ToString();
                    PROD_TIME = dt.Rows[0]["PROD_TIME"].ToString();
                    USER_NAME = dt.Rows[0]["USER_NAME"].ToString();
                    WO_NO = dt.Rows[0]["WO_NO"].ToString();
                    PROD_TYPE = dt.Rows[0]["PROD_TYPE"].ToString();
                    PROD_TYPE_NAME = dt.Rows[0]["PROD_TYPE_NAME"].ToString();
                    BAL_YN = dt.Rows[0]["BAL_YN"].ToString();
                    TEST_YN = dt.Rows[0]["TEST_YN"].ToString();
                    SAVE_TYPE = dt.Rows[0]["SAVE_TYPE"].ToString();
                    ENT_DT = dt.Rows[0]["ENT_DT"].ToString();
                    ENT_USER_ID = dt.Rows[0]["ENT_USER_ID"].ToString();
                    ITEM_NAME = dt.Rows[0]["ITEM_NAME"].ToString();
                    PROD_QTY = dt.Rows[0]["PROD_QTY"].ToString();
                    COMPOUND = dt.Rows[0]["COMPOUND"].ToString();
                    AGING_TIME = dt.Rows[0]["AGING_TIME"].ToString();
                    EXPIRY_DATE = dt.Rows[0]["EXPIRY_DATE"].ToString();
                    TREAD_WIDTH = dt.Rows[0]["TREAD_WIDTH"].ToString();
                    IO_POSID = dt.Rows[0]["IO_POSID"].ToString();
                    EQ_NAME = dt.Rows[0]["EQ_NAME"].ToString();
                    RCV_DT = dt.Rows[0]["RCV_DT"].ToString();
                    TEMP01 = dt.Rows[0]["TEMP01"].ToString();

                    for (int i = 1; i <= 10; i++)
                    {
                        if (!string.IsNullOrEmpty(dt.Rows[0]["COMMON_ITEM_ID_" + i].ToString()))
                            _COMMON_ITEM_ID.Add(dt.Rows[0]["COMMON_ITEM_ID_" + i].ToString());
                    }


                    if (!string.IsNullOrEmpty(LOT_ID))
                        GenerateBarcode();
                    else
                        LOT_ID_BARCODE_128 = "";

                   // LoadLayout();
                }
                
            }
        }

        private void GenerateBarcode()
        {
            try
            {
                var bar = new Barcode(LOT_ID,NetBarcode.Type.Code128B);
                //LOT_ID_BARCODE_128 = Convert.ToBase64String(bar.GetByteArray());
                LOT_ID_BARCODE_128 = bar.GetBase64Image();

            }
            catch (Exception ex)
            {
                LOT_ID_BARCODE_128 = "";
            }
        }

        /// <summary>
        /// Loads HTML layout internally. Use GetLayout() to load it
        /// </summary>
        public void LoadLayout()
        {
            if (!string.IsNullOrEmpty(_CART_ID)) 
            { 
                DataTable dt = _Database.SP_IN_H_PROD_LAYOUT(_CART_ID);
                if (dt.Rows.Count > 0)
                {
                    _LAYOUT = ReplaceClass(ReplaceLabels(dt.Rows[0]["LAYOUT"].ToString()));
                }
            }

        }

        /// <summary>
        /// Returns already loaded HTML layout. First use LoadLayout() to load it
        /// </summary>
        public string GetLayout()
        {
            return _LAYOUT;
        }

        private string ReplaceLabels(string LayoutRaw)
        {
            LayoutRaw = LayoutRaw.Replace("{{LOT_ID}}", LOT_ID);
            LayoutRaw = LayoutRaw.Replace("{{LOT_ID_BARCODE}}", LOT_ID_BARCODE_128);
            LayoutRaw = LayoutRaw.Replace("{{CART_ID}}", CART_ID);
            LayoutRaw = LayoutRaw.Replace("{{ITEM_NAME}}", ITEM_NAME);
            LayoutRaw = LayoutRaw.Replace("{{ITEM_ID}}", ITEM_ID) ;
            LayoutRaw = LayoutRaw.Replace("{{PROD_TIME}}", PROD_TIME);
            LayoutRaw = LayoutRaw.Replace("{{PROD_TYPE}}", PROD_TYPE);
            LayoutRaw = LayoutRaw.Replace("{{PROD_QTY}}", PROD_QTY);
            LayoutRaw = LayoutRaw.Replace("{{COMPOUND}}", COMPOUND);
            LayoutRaw = LayoutRaw.Replace("{{AGING_TIME}}", AGING_TIME);
            LayoutRaw = LayoutRaw.Replace("{{EXPIRY_DATE}}", EXPIRY_DATE);

            string width;
            if (TREAD_WIDTH.ToLower() != "none" && !string.IsNullOrEmpty(TREAD_WIDTH))
                width = TREAD_WIDTH + "mm";
            else 
                width = "";
            
            LayoutRaw = LayoutRaw.Replace("{{TREAD_WIDTH}}", width);
            
            LayoutRaw = LayoutRaw.Replace("{{IO_POSID}}", IO_POSID);
            LayoutRaw = LayoutRaw.Replace("{{EQ_NAME}}", EQ_NAME);
            LayoutRaw = LayoutRaw.Replace("{{RCV_DT}}", RCV_DT);
            LayoutRaw = LayoutRaw.Replace("{{USER_NAME}}", USER_NAME);
            LayoutRaw = LayoutRaw.Replace("{{TEMP01}}", TEMP01);

            string commonAll = "";
            foreach (string common in _COMMON_ITEM_ID)
                commonAll += common + " ";
                
            LayoutRaw = LayoutRaw.Replace("{{COMMON_ITEM_ID}}", commonAll);

            return LayoutRaw;
        }

        private string ReplaceClass(string LayoutRaw)
        {

            if (CART_STATE == "E")
                LayoutRaw = LayoutRaw.Replace("{{CLASS_DIV_CART_STATE}}", "bg-danger text-light");
            else
                LayoutRaw = LayoutRaw.Replace("{{CLASS_DIV_CART_STATE}}", "");

            return LayoutRaw;
        }

        private bool CheckInput()
        {
            if (string.IsNullOrEmpty(_CART_ID) && string.IsNullOrEmpty(_LOT_ID))
                return false;
            else
                return true;
        }

    }
}
