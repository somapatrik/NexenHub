using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using NexenHub.Class;
using Oracle.ManagedDataAccess.Client;

namespace NexenHub.Models
{
    public class Epaper
    {        
        private string _CART_ID;
        public string CART_ID 
        {
            get { return _CART_ID; } 
            set 
            { 
                _CART_ID = value;
                LoadFromDb();
            }
        }
        public string LOT_ID { get; set; }
        public string EVENT_TIME { get; set; }
        public string ITEM_ID { get; set; }
        public string WC_ID { get; set; }
        public string PROC_ID { get; set; }
        public string ITEM_STATE { get; set; }

        private GlobalDatabase _Database = new GlobalDatabase();

        public Epaper(){ }
        public Epaper(string CART_ID)
        {
            this.CART_ID = CART_ID;
        }

        private void LoadFromDb()
        {
            if (CheckInput())
            {
                DataTable dt = _Database.SP_DC_H_PROD_API_ESL(_CART_ID);
                LOT_ID = dt.Rows[0]["LOT_ID"].ToString();
                EVENT_TIME = dt.Rows[0]["EVENT_TIME"].ToString();
                ITEM_ID = dt.Rows[0]["ITEM_ID"].ToString();
                WC_ID = dt.Rows[0]["WC_ID"].ToString();
                PROC_ID = dt.Rows[0]["PROC_ID"].ToString();
                ITEM_STATE = dt.Rows[0]["ITEM_STATE"].ToString();
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
