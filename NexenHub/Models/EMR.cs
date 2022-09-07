using NexenHub.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace NexenHub.Models
{
    public class EMR
    {
        private string _PRD_REQ_NO;
        public string PRD_REQ_NO 
        { 
            get => _PRD_REQ_NO;
            set
            {
                _PRD_REQ_NO = value;
                LoadFromDB();
            } 
        }
        public string REQ_NO { get; set; }
        public string REQ_MEMBER { get; set; }
        public string REQ_DEPT { get; set; }
        public string TIRE_CODE { get; set; }
        public string PATTERN { get; set; }
        public string TIRE_SIZE { get; set; }
        public string MOLD_CONT { get; set; }
        public string BLADDER { get; set; }
        public string SWI_FLAG { get; set; }
        public string COMP_CSV { get; set; }
        public string TOOL { get; set; }
        public string PKX_TR { get; set; }
        public string PKX_SW { get; set; }
        public string BP_CORD { get; set; }
        public string SB_CORD { get; set; }
        public string CAP_CORD { get; set; }
        public string VER_CNT { get; set; }
        public string REQ_QTY { get; set; }
        public string WH_FLAG { get; set; }
        public string DEV_YMD { get; set; }
        public string CHASU { get; set; }
        public string TEST_TYPE { get; set; }
        public string REQ_TEXT { get; set; }
        public string TEST_SALES_CH { get; set; }
        public string REQ_YMD { get; set; }

        private GlobalDatabase dbglob = new GlobalDatabase();

        public EMR() { }
        public EMR(string prd_req_no)
        {
            PRD_REQ_NO = prd_req_no;
        }

        private void LoadFromDB()
        {
            DataTable dt = dbglob.GetEMRInfo(PRD_REQ_NO);
            if (dt.Rows.Count > 0)
            {
                REQ_NO = dt.Rows[0]["REQ_NO"].ToString();

                REQ_MEMBER = dt.Rows[0]["REQ_MEMBER"].ToString();
                REQ_DEPT = dt.Rows[0]["REQ_DEPT"].ToString();

                TIRE_CODE = dt.Rows[0]["TIRE_CODE"].ToString();
                PATTERN = dt.Rows[0]["PATTERN"].ToString();
                TIRE_SIZE = dt.Rows[0]["TIRE_SIZE"].ToString();
                MOLD_CONT = dt.Rows[0]["MOLD_CONT"].ToString();
                BLADDER = dt.Rows[0]["BLADDER"].ToString();
                SWI_FLAG = dt.Rows[0]["SWI_FLAG"].ToString();
                COMP_CSV = dt.Rows[0]["COMP_CSV"].ToString();
                TOOL = dt.Rows[0]["TOOL"].ToString();
                PKX_TR = dt.Rows[0]["PKX_TR"].ToString();
                PKX_SW = dt.Rows[0]["PKX_SW"].ToString();
                BP_CORD = dt.Rows[0]["BP_CORD"].ToString();
                SB_CORD = dt.Rows[0]["SB_CORD"].ToString();
                CAP_CORD = dt.Rows[0]["CAP_CORD"].ToString();
                VER_CNT = dt.Rows[0]["VER_CNT"].ToString();
                REQ_QTY = dt.Rows[0]["REQ_QTY"].ToString();
                WH_FLAG = dt.Rows[0]["WH_FLAG"].ToString();
                DEV_YMD = dt.Rows[0]["DEV_YMD"].ToString();
                CHASU = dt.Rows[0]["CHASU"].ToString();
                TEST_TYPE = dt.Rows[0]["TEST_TYPE"].ToString();
                REQ_TEXT = dt.Rows[0]["REQ_TEXT"].ToString();
                TEST_SALES_CH = dt.Rows[0]["TEST_SALES_CH"].ToString();
                REQ_YMD = dt.Rows[0]["REQ_YMD"].ToString();
            }
        }

    }
}
