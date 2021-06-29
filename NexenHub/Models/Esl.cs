using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using NexenHub.Class;
using Oracle.ManagedDataAccess.Client;

namespace NexenHub.Models
{
    public class Esl
    {
        private string _CART_ID;

        #region Public properties
        public string CART_ID 
        {
            get { return _CART_ID; } 
            set 
            { 
                _CART_ID = value.ToUpper();
                LoadFromDb();
            }
        }
        public string LOT_ID { get; set; }
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
        public string BAL_YN { get; set; }
        public string TEST_YN { get; set; }
        public string SAVE_TYPE { get; set; }
        public string ENT_DT { get; set; }
        public string ENT_USER_ID { get; set; }
        public string ITEM_NAME { get; set; }
        public bool VALID { get; set; }

        #endregion

        private GlobalDatabase _Database = new GlobalDatabase();

        public Esl(){ }
        public Esl(string CART_ID)
        {
            this.CART_ID = CART_ID;
        }

        private void LoadFromDb()
        {
            VALID = false;
            if (CheckInput())
            {
                DataTable dt = _Database.SP_DC_H_PROD_API_ESL(_CART_ID);

                if (dt.Rows.Count > 0)
                {
                    VALID = true;
                    LOT_ID = dt.Rows[0]["LOT_ID"].ToString();
                    EVENT_TIME = dt.Rows[0]["EVENT_TIME"].ToString();
                    ITEM_ID = dt.Rows[0]["ITEM_ID"].ToString();
                    WC_ID = dt.Rows[0]["WC_ID"].ToString();
                    PROC_ID = dt.Rows[0]["PROC_ID"].ToString();
                    ITEM_STATE = dt.Rows[0]["ITEM_STATE"].ToString();
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
                }
                
            }
        }

        private bool CheckInput()
        {
            if (!string.IsNullOrWhiteSpace(_CART_ID))
                return true;
            else
                return false;
        }

    }
}
