using NexenHub.Class;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Models
{
    public class Item
    {
        //private Settings settings = new Settings();
        private GlobalDatabase dbglobal = new GlobalDatabase();

        private string _ID;
        public string ITEM_NAME;
        public string ITEM_GRP;
        public string MATERIAL_GRP;
        public string ITEM_KIND;
        public string PROC_ID;
        public string ITEM_UNIT;
        public string BP_COMP;
        public string CORD_NAME;
        public string COMPOUND;
        public string PATTERN;
        public string SECTION_WIDTH;
        public string SERIES;
        public string INCH;
        public bool OE;


        public string ID
        {
            set
            {
                _ID = value;
                LoadFromDB();
            }

            get { return _ID; }
        }

        public Item()
        {

        }

        public Item(string ID)
        {
            _ID = ID;
            LoadFromDB();
        }

        private void LoadFromDB()
        {
            try
            {
                DataTable dt = dbglobal.LoadItem(_ID);
                if (dt.Rows.Count > 0)
                {
                    ITEM_NAME = dt.Rows[0]["ITEM_NAME"].ToString();
                    ITEM_GRP = dt.Rows[0]["ITEM_GRP"].ToString();
                    MATERIAL_GRP = dt.Rows[0]["MATERIAL_GRP"].ToString();
                    ITEM_KIND = dt.Rows[0]["ITEM_KIND"].ToString();
                    PROC_ID = dt.Rows[0]["PROC_ID"].ToString();
                    ITEM_UNIT = dt.Rows[0]["ITEM_UNIT"].ToString();
                    BP_COMP = dt.Rows[0]["BP_COMP"].ToString();
                    CORD_NAME = dt.Rows[0]["CORD_NAME"].ToString();
                    COMPOUND = dt.Rows[0]["COMPOUND"].ToString();


                    PATTERN = dt.Rows[0]["PATTERN"].ToString();
                    SECTION_WIDTH = dt.Rows[0]["SECTION_WIDTH"].ToString();
                    SERIES = dt.Rows[0]["SERIES"].ToString();
                    INCH = dt.Rows[0]["INCH"].ToString();
                    OE = dt.Rows[0]["OE"].ToString() == "Y";


                }

            }
            catch
            {

            }
        }

    }
}
