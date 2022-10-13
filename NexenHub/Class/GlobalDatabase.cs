using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NexenHub.Class;
using Oracle.ManagedDataAccess.Client;

namespace NexenHub.Class
{
    public class GlobalDatabase
    {
        public void UpdateVersion(string appID, string IP, string VersionName)
        {
            try
            {
                DBOra db = new DBOra("SP_CM_M_VERSION_UPDATE");
                db.AddParameter("AS_IP", IP, OracleDbType.Varchar2);
                db.AddParameter("AS_SOFTWARE_ID", appID, OracleDbType.Varchar2);
                db.AddParameter("AS_VERSION_NAME", VersionName, OracleDbType.Varchar2);

                db.AddOutput("RC_TABLE", OracleDbType.RefCursor);
                db.AddOutput("RS_CODE", OracleDbType.Varchar2, 100);
                db.AddOutput("RS_MSG", OracleDbType.Varchar2, 100);

                db.ExecProcedure();

            }
            catch (Exception ex)
            {

            }
        }

        public List<string> GetCommonGt(string LOT_ID, string ITEM_ID = "")
        {
            List<string> results = new List<string>();

            try
            {
                DBOra db = new DBOra("SP_CM_M_BOM_COMMON_GT");
                db.AddParameter("AS_ITEM_ID", ITEM_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_LOT_ID", LOT_ID, OracleDbType.Varchar2);

                db.AddOutput("RC_TABLE", OracleDbType.RefCursor);
                db.AddOutput("RS_CODE", OracleDbType.Varchar2, 100);
                db.AddOutput("RS_MSG", OracleDbType.Varchar2, 100);

                DataTable dt = db.ExecProcedure();
                foreach (DataRow row in dt.Rows)
                    results.Add(row["GT_ITEM_ID"].ToString());
            } 
            catch (Exception ex)
            {
               
            }

            return results;
        }

        public string GetExtCompound(string LOT_ID)
        {
            try
            {
                DBOra db = new DBOra("select FN_CM_GET_EXT_COMPOUND(:lot) FROM DUAL");
                db.AddParameter("lot", LOT_ID, OracleDbType.Varchar2);
                DataTable dt = db.ExecTable();

                if (dt.Rows.Count > 0)
                    return dt.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {

            }

            return null;
        }


        public string GetTreadWidth(string LOT_ID)
        {
            try
            {
                DBOra db = new DBOra("select FN_CM_GET_TREAD_WIDTH(:lot) FROM DUAL");
                db.AddParameter("lot", LOT_ID, OracleDbType.Varchar2);
                DataTable dt = db.ExecTable();
            
                if (dt.Rows.Count > 0)
                    return dt.Rows[0][0].ToString();
            } 
            catch (Exception ex)
            {
                
            }

            return null;
        }

        public DataTable GetShiftDownTimes(string EQ_ID, DateTime StartTime, DateTime EndTime)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT ");
                query.AppendLine("TO_DATE(NONWRK_STIME, 'YYYYMMDDHH24MISS') STIME, ");
                query.AppendLine("TO_DATE(NVL(NONWRK_ETIME, to_char(SYSDATE, 'YYYYMMDDHH24MISS')), 'YYYYMMDDHH24MISS') ETIME, ");
                query.AppendLine("NON.NONWRK_CODE, ");
                query.AppendLine("CODE.NONWRK_NAME_1033 NONWRK_NAME");
                query.AppendLine("FROM TB_CM_M_NONWRK NON");
                query.AppendLine("LEFT JOIN TB_CM_M_NONWRKCODE CODE on CODE.NONWRK_CODE = NON.NONWRK_CODE");
                query.AppendLine("WHERE EQ_ID = :EQ_ID");
                query.AppendLine("AND((NONWRK_ETIME >= :STIME AND NONWRK_STIME <= :ETIME)");
                query.AppendLine("OR(NONWRK_ETIME IS NULL AND NONWRK_STIME IS NOT NULL))");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("EQ_ID", EQ_ID, OracleDbType.Varchar2);
                db.AddParameter("STIME", StartTime.ToString("yyyyMMddHHmmss"), OracleDbType.Varchar2);
                db.AddParameter("ETIME", EndTime.ToString("yyyyMMddHHmmss"), OracleDbType.Varchar2);
                return db.ExecTable();

            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable GetShiftInfo()
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("select");
                query.AppendLine("SHIFT,");
                query.AppendLine("SHIFT_RGB,");
                query.AppendLine("to_date(START_DT, 'YYYYMMDDHH24MISS') STIME,");
                query.AppendLine("to_date(END_DT, 'YYYYMMDDHH24MISS') ETIME,");
                query.AppendLine("((to_date(END_DT, 'YYYYMMDDHH24MISS') - to_date(START_DT, 'YYYYMMDDHH24MISS')) * 24 * 60 * 60) REAL_WORK_SEC,");
                query.AppendLine("(WEEK) W");
                query.AppendLine("from TB_CM_M_CALENDAR");
                query.AppendLine("WHERE(TO_CHAR(SYSDATE, 'YYYYMMDDHH24MISS') BETWEEN START_DT AND END_DT)");
                query.AppendLine("AND SHIFT IN('A', 'C')");

                DBOra db = new DBOra(query.ToString());
                return db.ExecTable();

            }
            catch(Exception ex)
            {
                return new DataTable();
            }
        }

        public DataRow Pos2MiniPc(string POSID, string EQ_ID)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT MINIPC_ID");
                query.AppendLine("FROM TB_CM_M_MONITORING_READER");
                query.AppendLine("WHERE POSID = :pos");
                query.AppendLine("AND EQ_ID = :eq");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("pos", POSID, OracleDbType.Varchar2);
                db.AddParameter("eq", EQ_ID, OracleDbType.Varchar2);

                DataTable dt = db.ExecTable();
                if (dt.Rows.Count > 0)
                    return dt.Rows[0];
            }
            catch (Exception ex)
            {
               
            }

            return null;
        }

        public DataTable GetUsedHalb(string barcode)
        {
            try
            {
                DBOra db = new DBOra("SP_PR_MR_TIRE_HALB_INFO");
                db.AddParameter("AS_TIRE_NO", barcode, OracleDbType.Varchar2);

                db.AddOutput("RC_TABLE", OracleDbType.RefCursor);
                db.AddOutput("RS_CODE", OracleDbType.Varchar2, 100);
                db.AddOutput("RS_MSG", OracleDbType.Varchar2, 100);

                DataTable dt = db.ExecProcedure();
                return dt;
            
            } 
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable GetEMRInfo(string PRD_REQ_NO)
        {
            try 
            {
                DBOra db = new DBOra("SP_PL_MR_TEST_REQ_INFO");
                db.AddParameter("AS_REQ_NO", PRD_REQ_NO, OracleDbType.Varchar2);

                db.AddOutput("RC_TABLE", OracleDbType.RefCursor);
                db.AddOutput("RS_CODE", OracleDbType.Varchar2, 100);
                db.AddOutput("RS_MSG", OracleDbType.Varchar2, 100);

                DataTable dt = db.ExecProcedure();
                return dt;
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable GetFertInspectionResult(string barcode)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT");
                query.AppendLine("INSP_SEQ,");
                query.AppendLine("FN_CM_GET_CODE_NAME('QA', '01', PROC_ID, '1033') AS PROC_ID,");
                query.AppendLine("TO_DATE(INSP_DT, 'YYYYMMDDHH24MISS') AS INSPECTION_TIME,");
                query.AppendLine("SHIFT,");
                query.AppendLine("SHIFT_RGB,");
                query.AppendLine("BAD_ID,");
                query.AppendLine("BAD_GRADE,");
                query.AppendLine("LOC_MOLD,");
                query.AppendLine("LOC_SIDE,");
                query.AppendLine("LOC_ZONE,");
                query.AppendLine("LOC_POSITION,");
                query.AppendLine("CQ2,");
                query.AppendLine("FN_CM_GET_USER_INFO(PLANT_ID, ENT_USER_ID, 'MEMBER_NAME') AS ENT_USER");
                query.AppendLine("FROM");
                query.AppendLine("TB_QA_H_PROC_BAD_DETAIL");
                query.AppendLine("WHERE");
                query.AppendLine("BARCODE_NO = :barcode");
                query.AppendLine("ORDER BY 3");


                DBOra db = new DBOra(query.ToString());
                db.AddParameter("barcode", barcode, OracleDbType.Varchar2);
                DataTable dt = db.ExecTable();
                return dt;
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
}

        public string Cart2Lot(string CART_ID)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT PROD.LOT_ID");
                query.AppendLine("FROM TB_PR_M_PROD PROD");
                query.AppendLine("WHERE PROD.CART_ID = :cart");
                query.AppendLine("AND PROD.PROD_TIME = (SELECT MAX(PROD_TIME) FROM TB_PR_M_PROD WHERE CART_ID = :cart AND USE_YN = 'Y')");
                query.AppendLine("AND PROD.USE_YN = 'Y'");
                query.AppendLine("AND PROD.PLANT_ID = 'P500'");

                DBOra db = new DBOra("SELECT FN_IN_CART_TO_LOT(:cart) FROM DUAL ");
                db.AddParameter("cart", CART_ID.ToUpper(), OracleDbType.Varchar2);
                DataTable dt = db.ExecTable();
                return dt.Rows.Count > 0 ? dt.Rows[0][0].ToString() : "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public int GetICSCount()
        {
            try
            {
                DBOra db = new DBOra("SELECT COUNT(*) FROM TB_CM_M_MONITORING_CONFIG WHERE USE_YN = 'Y'");
                DataTable dt = db.ExecTable();
                return dt.Rows.Count > 0 ? int.Parse(dt.Rows[0][0].ToString()) : 0;

            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int GetRexCount()
        {
            try
            {
                DBOra db = new DBOra("SELECT COUNT(*) FROM TB_CM_M_VERSION WHERE SOFTWARE_ID = 'rex'");
                DataTable dt = db.ExecTable();
                return dt.Rows.Count > 0 ? int.Parse(dt.Rows[0][0].ToString()) : 0;

            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int GetIOServerCount()
        {
            try
            {
                DBOra db = new DBOra("SELECT COUNT(*) FROM TB_CM_M_NMP_SETTINGS WHERE USE_YN = 'Y'");
                DataTable dt = db.ExecTable();
                return dt.Rows.Count > 0 ? int.Parse(dt.Rows[0][0].ToString()) : 0;

            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public DataTable GetLatestSoftwareCount(string SOFTWARE_ID)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT COUNT(*) LATESTCOUNT, VERSION_NAME ");
                query.AppendLine("FROM TB_CM_M_VERSION");
                query.AppendLine("WHERE SOFTWARE_ID = :soft");
                query.AppendLine("AND VERSION_NAME = (SELECT MAX(VERSION_NAME) FROM TB_CM_M_VERSION WHERE SOFTWARE_ID = :soft)");
                query.AppendLine("GROUP BY VERSION_NAME");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("soft", SOFTWARE_ID, OracleDbType.Varchar2);
                DataTable dt = db.ExecTable();
                return dt;

            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable LoadItem(string ID)
        {
            try
            {
                DBOra db = new DBOra("SP_MON_ITEM");
                db.AddParameter("AS_PLANT_ID", GlobalSettings.PLANT_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_ITEM_ID", ID, OracleDbType.Varchar2);
                db.AddOutput("RC_TABLE", OracleDbType.RefCursor);
                db.AddOutput("RS_CODE", OracleDbType.Varchar2, 100);
                db.AddOutput("RS_MSG", OracleDbType.Varchar2, 100);
                DataTable dt = db.ExecProcedure();
                return dt;

            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable BarcodeToLotId(string code)
        {
            try
            {
                

                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT");
                query.AppendLine("VMI_BARCODE BARCODE,");
                query.AppendLine("VMI_LOT_ID TBM_LOT_ID,");
                query.AppendLine("CURE_LOT_ID CURE_LOT_ID");
                query.AppendLine("FROM TB_IN_M_BARCODE_TRACE");
                query.AppendLine("where (VMI_BARCODE = :code AND LENGTH(:code) = 10) ");
                query.AppendLine("OR ((VMI_LOT_ID = :code OR CURE_LOT_ID = :code) AND LENGTH(:code) = 15)");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("code", code, OracleDbType.Varchar2);
                DataTable dt = db.ExecTable();
                return dt;
            } 
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable GetDefectMonitoringPicture(string WC_ID)
        {
            try
            {

                StringBuilder query = new StringBuilder();
                query.AppendLine("select BAD_ID, IMG_NAME, IMG_PATH");
                query.AppendLine("from TB_QA_M_DEFECT_MON");
                query.AppendLine("where DISPLAY_YN = 'Y' and WC_ID=:wc");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("wc",WC_ID, OracleDbType.Varchar2);
                DataTable dt = db.ExecTable();
                return dt;

            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable GetTbmPlan(string EQ_ID)
        {
            try
            {
                DBOra db = new DBOra("SP_DS_MP_TBM_MONITOR_DEFECT");
               // db.AddParameter("AS_PLANT_ID", GlobalSettings.PLANT_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_EQ_ID", EQ_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_SHIFT_DATE",null, OracleDbType.Date);

                db.AddOutput("RC_TABLE", OracleDbType.RefCursor);
                db.AddOutput("RS_CODE", OracleDbType.Varchar2, 100);
                db.AddOutput("RS_MSG", OracleDbType.Varchar2, 100);

                DataTable dt = db.ExecProcedure();
                return dt;

            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable IsMemberPresent(string member_id)
        {
            try
            {

                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT STATUS");
                query.AppendLine("FROM VIEW_PRESLIST_OSOBA");
                query.AppendLine("WHERE OSOBNICISLO = @memberid");

                DBMic db = new DBMic(query.ToString(), DBMic.Database.Aktion);
                db.AddParameter("memberid", member_id, SqlDbType.VarChar);
                DataTable dt = db.ExecTable();
                return dt;

            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable GetMemberSearch()
        {
            try
            {

                StringBuilder query = new StringBuilder();
                query.AppendLine("select distinct MEMBER_ID, MEMBER_NAME, MEM.EMAIL_ADDR EMAIL, BOOK.RCV_PHN_ID PHONE");
                query.AppendLine("from TB_CM_M_MEMBER mem");
                query.AppendLine("left join TB_CM_M_CALLBOOK book on BOOK.TEMP01 = MEM.MEMBER_ID");
                query.AppendLine("where mem.PLANT_ID = 'P500'");
                query.AppendLine("AND mem.USE_YN = 'Y'");
                query.AppendLine("AND EMP_STATS = 1 ");
                query.AppendLine("ORDER BY MEMBER_ID ");

                DBOra db = new DBOra(query.ToString());
                DataTable dt = db.ExecTable();
                return dt;

            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable GetMember(string ID)
        {
            try
            {

                StringBuilder query = new StringBuilder();
                query.AppendLine("select /*+ index(mem IX_CM_M_MEMBER_3)*/");
                query.AppendLine("mem.MEMBER_ID,");
                query.AppendLine("mem.MEMBER_NAME,");
                query.AppendLine("mem.TTOUT1 POSITION,");
                query.AppendLine("dept.DEPT_ID,");
                query.AppendLine("dept.DEPT_NAME,");
                query.AppendLine("head.DEPT_ID HEAD_DEPT_ID,");
                query.AppendLine("head.DEPT_NAME HEAD_DEPT_NAME,");
                query.AppendLine("RCV_PHN_ID PHONE,");
                query.AppendLine("lower(MEM.EMAIL_ADDR) EMAIL");
                query.AppendLine("from TB_CM_M_MEMBER mem");
                query.AppendLine("left join TB_CM_M_DEPT dept on DEPT.DEPT_ID = mem.DEPT_ID");
                query.AppendLine("left join TB_CM_M_DEPT head on head.DEPT_ID = DEPT.UP_DEPT_ID");
                query.AppendLine("left join TB_CM_M_CALLBOOK book on book.TEMP01 = mem.MEMBER_ID");
                query.AppendLine("where mem.MEMBER_ID = :id");
                query.AppendLine("and mem.EMP_STATS = 1");
                query.AppendLine("and mem.USE_YN = 'Y' ");
                query.AppendLine("and mem.PLANT_ID='P500' ");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("id", ID, OracleDbType.NVarchar2);
                DataTable dt = db.ExecTable();
                return dt;

            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable LoadWorkOrderList(string EQ_ID)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT /*+ INDEX(TB_PL_M_WRKORD IX_PL_M_WRKORD_5)*/");
                query.AppendLine("WO_NO,");
                query.AppendLine("to_date(WO_STIME,'YYYYMMDDHH24MISS') WO_STIME,");
                query.AppendLine("to_date(WO_ETIME,'YYYYMMDDHH24MISS') WO_ETIME,");
                query.AppendLine("to_date(MIXER_PROD_STIME,'YYYYMMDDHH24MISS') PLAN_STIME,");
                query.AppendLine("to_date(MIXER_PROD_ETIME,'YYYYMMDDHH24MISS') PLAN_ETIME,");
                query.AppendLine("PROD_TYPE,");
                query.AppendLine("wo.ITEM_ID,");
                query.AppendLine("item.ITEM_NAME,");
                query.AppendLine("WO_QTY,");
                query.AppendLine("PROD_QTY,");
                query.AppendLine("UNIT,");
                query.AppendLine("TEST_YN,");
                query.AppendLine("PROTOTYPE_ID,");
                query.AppendLine("PROTOTYPE_VER,");
                query.AppendLine("DEL_FLAG,");
                query.AppendLine("WO_PROC_STATE");
                query.AppendLine("FROM TB_PL_M_WRKORD wo");
                query.AppendLine("LEFT JOIN TB_CM_M_ITEM item on wo.ITEM_ID=item.ITEM_ID");
                query.AppendLine("WHERE wo.PLANT_ID = 'P500'");
                query.AppendLine("AND EQ_ID = :eq");
                query.AppendLine("AND wo.USE_YN = 'Y'");
                query.AppendLine("AND (WO_PROC_STATE IN('W', 'S')");
                query.AppendLine("OR WO_PROC_STATE = 'F' AND (TRUNC(SYSDATE - (6/24)) - (to_date(WO_ETIME,'YYYYMMDDHH24MISS')-(6/24)) BETWEEN -1 AND 1))");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("eq", EQ_ID, OracleDbType.NVarchar2);
                DataTable dt = db.ExecTable();
                return dt;

            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }
        public DataTable LoadWorkOrderByWo(string WO)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("select");
                query.AppendLine("WO_NO,");
                query.AppendLine("to_date(WO_STIME,'yyyymmddhh24miss') WO_STIME,"); 
                query.AppendLine("to_date(WO_ETIME,'yyyymmddhh24miss') WO_ETIME,");
                query.AppendLine("to_date(MIXER_PROD_STIME,'YYYYMMDDHH24MISS') PLAN_STIME,");
                query.AppendLine("to_date(MIXER_PROD_ETIME,'YYYYMMDDHH24MISS') PLAN_ETIME,");
                query.AppendLine("PROD_TYPE,");
                query.AppendLine("ITEM_ID,");
                query.AppendLine("WO_QTY,");
                query.AppendLine("PROD_QTY,");
                query.AppendLine("UNIT,");
                query.AppendLine("TEST_YN,");
                query.AppendLine("PROTOTYPE_ID,");
                query.AppendLine("PROTOTYPE_VER,");
                query.AppendLine("DEL_FLAG");
                query.AppendLine("from TB_PL_M_WRKORD");
                query.AppendLine("where WO_NO = :wo");
                query.AppendLine("and PLANT_ID = 'P500'");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("wo", WO, OracleDbType.NVarchar2);
                DataTable dt = db.ExecTable();
                return dt;

            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }
        public DataTable LoadWOChildItem(string ID, string MiniPC)
        {
            try
            {
                DBOra db = new DBOra("SP_MON_GET_CURRENT_CHILD_ITEM");
                db.AddParameter("AS_PLANT_ID", GlobalSettings.PLANT_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_ITEM_ID", ID, OracleDbType.Varchar2);
                db.AddParameter("AS_MINIPC_ID", MiniPC, OracleDbType.Varchar2);

                db.AddOutput("RC_TABLE", OracleDbType.RefCursor);
                db.AddOutput("RS_CODE", OracleDbType.Varchar2, 100);
                db.AddOutput("RS_MSG", OracleDbType.Varchar2, 100);
                DataTable dt = db.ExecProcedure();
                return dt;

            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }
        public DataTable GetPremiumGtInfo(string ITEM_ID)
        {
            try
            {
                DBOra db = new DBOra("SP_MO_MR_PRIMIEUMOE_INFO");

                db.AddParameter("AS_PLANT_ID", GlobalSettings.PLANT_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_CURRENT_ITEM_ID", ITEM_ID, OracleDbType.Varchar2);

                db.AddOutput("RC_TABLE", OracleDbType.RefCursor);
                db.AddOutput("RS_CODE", OracleDbType.Varchar2, 100);
                db.AddOutput("RS_MSG", OracleDbType.Varchar2, 100);

                DataTable dt = db.ExecProcedure();
                return dt;
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable NoLifeCheck(string LOT_ID)
        {
            try
            {
                DBOra db = new DBOra("SP_MO_MR_NOLIFE_CHECK");

                db.AddParameter("AS_LOT_ID", LOT_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_PLANT_ID", GlobalSettings.PLANT_ID, OracleDbType.Varchar2);

                db.AddOutput("RC_TABLE", OracleDbType.RefCursor);
                db.AddOutput("RS_CODE", OracleDbType.Varchar2, 100);
                db.AddOutput("RS_MSG", OracleDbType.Varchar2, 100);

                DataTable dt = db.ExecProcedure();
                return dt;
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }
        public DataTable NoValidCheck(string EQ_ID, string LOT_ID, string WO_NO, string BOM_ITEM_ID, string BOM_ITEM_NAME, string BOM_ITEM_COMPOUND)
        {
            try
            {
                DBOra db = new DBOra("SP_MO_MR_NOVALID_CHECK3");

                db.AddParameter("AS_PLANT_ID", GlobalSettings.PLANT_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_EQ_ID", EQ_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_LOT_ID", LOT_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_WO_NO", WO_NO, OracleDbType.Varchar2);

                db.AddParameter("AS_BOM_ITEM_ID", BOM_ITEM_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_BOM_ITEM_NAME", BOM_ITEM_NAME, OracleDbType.Varchar2);
                db.AddParameter("AS_BOM_COMPOUND", BOM_ITEM_COMPOUND, OracleDbType.Varchar2);

                db.AddOutput("RC_TABLE", OracleDbType.RefCursor);
                db.AddOutput("RS_CODE", OracleDbType.Varchar2, 100);
                db.AddOutput("RS_MSG", OracleDbType.Varchar2, 100);

                DataTable dt = db.ExecProcedure();
                return dt;
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }
        public DataTable FIFOCheck(string LOT_ID)
        {
            try
            {
                DBOra db = new DBOra("SP_MO_MR_FIFO_CHECK_2");

                db.AddParameter("AS_LOT_ID", LOT_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_PLANT_ID", GlobalSettings.PLANT_ID, OracleDbType.Varchar2);

                db.AddOutput("RC_TABLE", OracleDbType.RefCursor);
                db.AddOutput("RS_CODE", OracleDbType.Varchar2, 100);
                db.AddOutput("RS_MSG", OracleDbType.Varchar2, 100);

                DataTable dt = db.ExecProcedure();
                return dt;
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }
        public DataTable AgingCheck(string LOT_ID)
        {
            try
            {
                DBOra db = new DBOra("SP_MO_MR_AGING_CHECK");

                db.AddParameter("AS_LOT_ID", LOT_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_PLANT_ID", GlobalSettings.PLANT_ID, OracleDbType.Varchar2);

                db.AddOutput("RC_TABLE", OracleDbType.RefCursor);
                db.AddOutput("RS_CODE", OracleDbType.Varchar2, 100);
                db.AddOutput("RS_MSG", OracleDbType.Varchar2, 100);

                DataTable dt = db.ExecProcedure();
                return dt;
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        /// <summary>
        /// ICS query for getting WO
        /// </summary>
        /// <returns></returns>
        public DataTable LoadWorkOrder(string EQ_ID, string FACT_ID = "NEX1")
        {
            try
            {
                DBOra db = new DBOra("SP_MON_WORKORDER");
                db.AddParameter("AS_PLANT_ID", GlobalSettings.PLANT_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_FACT_ID", FACT_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_EQ_ID", EQ_ID, OracleDbType.Varchar2);
                db.AddOutput("RC_TABLE", OracleDbType.RefCursor);
                db.AddOutput("RS_CODE", OracleDbType.Varchar2, 100);
                db.AddOutput("RS_MSG", OracleDbType.Varchar2, 100);
                DataTable dt = db.ExecProcedure();
                return dt;
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable GetLotHis(string LOT_ID)
        {
            try
            {
                // Jo, je to natvrdo
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT ");
                query.AppendLine("to_date(HIS.TRAN_TIME, 'YYYYMMDDHH24MISS') TRANDATE,");
                query.AppendLine("LOC.LOC_DESC_1033 LOCATION,");
                query.AppendLine("CASE");
                query.AppendLine("WHEN HIS.LOT_STATE = 'A' THEN '(A) Created'");
                query.AppendLine("WHEN HIS.LOT_STATE = 'B' THEN '(B) Loading'");
                query.AppendLine("WHEN HIS.LOT_STATE = 'C' THEN '(C) Wait for relocation'");
                query.AppendLine("WHEN HIS.LOT_STATE = 'D' THEN '(D) Relocation'");
                query.AppendLine("WHEN HIS.LOT_STATE = 'E' THEN '(E) Wait for delivery'");
                query.AppendLine("WHEN HIS.LOT_STATE = 'F' THEN '(F) Delivery'");
                query.AppendLine("WHEN HIS.LOT_STATE = 'G' THEN '(G) Wait for input'");
                query.AppendLine("WHEN HIS.LOT_STATE = 'H' THEN '(H) Input'");
                query.AppendLine("WHEN HIS.LOT_STATE = 'Z' THEN '(Z) Completed'");
                query.AppendLine("ELSE HIS.LOT_STATE");
                query.AppendLine("END LOTSTATE,");
                query.AppendLine("CASE");
                query.AppendLine("WHEN HIS.ITEM_STATE = 'N' THEN 'Normal'");
                query.AppendLine("WHEN HIS.ITEM_STATE = 'B' THEN 'Bad'");
                query.AppendLine("WHEN HIS.ITEM_STATE = 'H' THEN 'Hold'");
                query.AppendLine("WHEN HIS.ITEM_STATE = 'S' THEN 'Scrap'");
                query.AppendLine("ELSE +HIS.ITEM_STATE");
                query.AppendLine("END ITEMSTATE,");
                query.AppendLine("(HIS.CURRENT_QTY || ' ' || HIS.UNIT) QTY");
                query.AppendLine("from TB_IN_H_LOTHIS HIS");
                query.AppendLine("left");
                query.AppendLine("join TB_IN_M_LOC loc on LOC.LOC_ID = HIS.LOC_NO");
                query.AppendLine("where LOT_ID = :lot");
                query.AppendLine("order by TRAN_TIME");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("lot", LOT_ID, OracleDbType.Varchar2);

                DataTable dt = db.ExecTable();
                return dt;
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        /// <summary>
        /// Without status translation
        /// </summary>
        /// <param name="LOT_ID"></param>
        /// <returns></returns>
        public DataTable GetLotHisClean(string LOT_ID)
        {
            try
            {
                // Jo, je to natvrdo
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT ");
                query.AppendLine("to_date(HIS.TRAN_TIME, 'YYYYMMDDHH24MISS') TRANDATE,");
                query.AppendLine("LOC.LOC_DESC_1033 LOCATION,");
                query.AppendLine("HIS.LOT_STATE LOTSTATE,");
                query.AppendLine("HIS.ITEM_STATE ITEMSTATE,");
                query.AppendLine("(HIS.CURRENT_QTY || ' ' || HIS.UNIT) QTY");
                query.AppendLine("from TB_IN_H_LOTHIS HIS");
                query.AppendLine("left");
                query.AppendLine("join TB_IN_M_LOC loc on LOC.LOC_ID = HIS.LOC_NO");
                query.AppendLine("where LOT_ID = :lot");
                query.AppendLine("order by TRAN_TIME");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("lot", LOT_ID, OracleDbType.Varchar2);

                DataTable dt = db.ExecTable();
                return dt;
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable GetLotInfo(string LOT_ID)
        {
            try
            {
                DBOra db = new DBOra("SP_MO_MR_INPUT_ITEM_INFO_2");

                db.AddParameter("AS_PLANT_ID", "P500", OracleDbType.Varchar2);
                db.AddParameter("AS_LOT_ID", LOT_ID, OracleDbType.Varchar2);

                db.AddOutput("RC_TABLE", OracleDbType.RefCursor);
                db.AddOutput("RS_CODE", OracleDbType.Varchar2, 100);
                db.AddOutput("RS_MSG", OracleDbType.Varchar2, 100);

                DataTable dt = db.ExecProcedure();
                return dt;
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable GetPingDevices()
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("select 'NMP: ' || DESCRIPTION DISPLAYNAME, IP_ADDRESS IP from TB_CM_M_NMP_SETTINGS where USE_YN='Y'");
                query.AppendLine("UNION ALL");
                query.AppendLine("select 'ICS: ' || DISPLAYNAME DISPLAYNAME, IP from TB_CM_M_MONITORING_CONFIG where USE_YN = 'Y' ");

                DBOra db = new DBOra(query.ToString());

                return db.ExecTable();
            }
            catch(Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable MachineReportDownTimes(string EQ_ID, DateTime start, DateTime end)
        {
            try
            {
                string startDT = start.ToString("yyyyMMddHHmmss");
                string endDT = end.ToString("yyyyMMddHHmmss");

                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT ");
                query.AppendLine("TO_DATE(NONWRK_STIME, 'YYYYMMDDHH24MISS') STIME,");
                query.AppendLine("TO_DATE(NVL(NONWRK_ETIME, to_char(SYSDATE, 'YYYYMMDDHH24MISS')), 'YYYYMMDDHH24MISS') ETIME,");
                query.AppendLine("NON.NONWRK_CODE,");
                query.AppendLine("CODE.NONWRK_NAME_1033 NONWRK_NAME");
                query.AppendLine("FROM TB_CM_M_NONWRK NON");
                query.AppendLine("LEFT JOIN TB_CM_M_NONWRKCODE CODE on CODE.NONWRK_CODE = NON.NONWRK_CODE");
                query.AppendLine("WHERE EQ_ID = :eqid");
                query.AppendLine("AND(NONWRK_ETIME >= :startDT AND NONWRK_STIME <= :endDT)");
                query.AppendLine("OR NONWRK_STIME IS NULL");
                query.AppendLine("ORDER BY NONWRK_STIME");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("eqid", EQ_ID, OracleDbType.Varchar2);
                db.AddParameter("startDT", startDT, OracleDbType.Varchar2);
                db.AddParameter("endDT", endDT, OracleDbType.Varchar2);

                return db.ExecTable();
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable GetJandiMsg()
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT");
                query.AppendLine("SND_TITLE TITLE,");
                query.AppendLine("SND_MSG MSG");
                query.AppendLine("FROM TB_DC_H_MESSAGE_JANDI");
                query.AppendLine("WHERE SND_FLAG = 'Y'");
                query.AppendLine("AND MSG_TYPE = 'PRD'");
                query.AppendLine("AND ENT_DT = (select MAX(ENT_DT) from TB_DC_H_MESSAGE_JANDI)");
                query.AppendLine("AND ROWNUM = 1");
                query.AppendLine("ORDER BY ENT_DT desc");

                DBOra db = new DBOra(query.ToString());

                return db.ExecTable();
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable MachineReportWorkOrders(string EQ_ID, DateTime start, DateTime end)
        {
            try
            {
                // For test converted here - I am pretty sure this will stay here forever - GUBUN
                string startDT = start.ToString("yyyyMMddHHmmss");
                string endDT = end.ToString("yyyyMMddHHmmss");

                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT WO_NO, to_date(WO_STIME,'YYYYMMDDHH24MISS') WO_STIME, to_date(WO_ETIME,'YYYYMMDDHH24MISS') WO_ETIME, WO.ITEM_ID, ITEM.ITEM_NAME ");
                query.AppendLine("FROM TB_PL_M_WRKORD WO ");
                query.AppendLine("LEFT JOIN TB_CM_M_ITEM ITEM ON ITEM.ITEM_ID = WO.ITEM_ID ");
                query.AppendLine("WHERE WO.PLANT_ID = :plant ");
                query.AppendLine("AND WO.EQ_ID = :eqid ");
                query.AppendLine("AND WO.USE_YN = 'Y' ");
                query.AppendLine("AND WO.DEL_FLAG = 'N' ");
                query.AppendLine("AND (WO.WO_STIME >= :startDT OR WO.WO_ETIME >= :startDT2) ");
                query.AppendLine("AND (WO.WO_STIME <= :endDT OR WO.WO_ETIME <= :endDT2) ");
                
                DBOra db = new DBOra(query.ToString());
                db.AddParameter("plant", "P500", OracleDbType.Varchar2);
                db.AddParameter("eqid", EQ_ID, OracleDbType.Varchar2);
                db.AddParameter("startDT", startDT, OracleDbType.Varchar2);
                db.AddParameter("startDT2", startDT, OracleDbType.Varchar2);
                db.AddParameter("endDT", endDT, OracleDbType.Varchar2);
                db.AddParameter("endDT2", endDT, OracleDbType.Varchar2);

                return db.ExecTable();
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable MachineReportUsedMat(string EQ_ID, DateTime start, DateTime end)
        {
            try
            {
                // For test converted here - I am pretty sure this will stay here forever - GUBUN
                string startDT = start.ToString("yyyyMMddHHmmss");
                string endDT = end.ToString("yyyyMMddHHmmss");


                StringBuilder query = new StringBuilder();

                query.AppendLine("SELECT /*+INDEX(TB_EQ_H_EQPOSHIS IX_EQ_M_EQPOSHIS_IDX_02)*/ ");
                query.AppendLine("INP.IO_POSID, INP.IO_POSGB, INP.CART_ID, INP.LOT_ID, INP.ENT_DT,INP.ITEM_ID,ITEM.ITEM_NAME ");
                query.AppendLine("FROM TB_EQ_H_EQPOSHIS INP ");
                query.AppendLine("LEFT JOIN TB_CM_M_ITEM ITEM ON ITEM.ITEM_ID = INP.ITEM_ID ");
                query.AppendLine("WHERE INP.LOT_ID in( ");
                query.AppendLine("SELECT /*+INDEX(TB_EQ_H_EQPOSHIS IX_EQ_M_EQPOSHIS_IDX_01)*/ ");
                query.AppendLine("DISTINCT(LOT_ID) ");
                query.AppendLine("FROM TB_EQ_H_EQPOSHIS lot ");
                query.AppendLine("WHERE lot.EVENT_TIME >= :startDT AND lot.EVENT_TIME <= :endDT ");
                query.AppendLine("AND lot.EQ_ID = :eqid ");
                query.AppendLine("AND lot.PLANT_ID = :plant ");
                query.AppendLine("AND lot.IO_POSID is not null) ");
                query.AppendLine("ORDER BY IO_POSID, INP.ENT_DT, INP.LOT_ID ");


                DBOra db = new DBOra(query.ToString());
                db.AddParameter("startDT", startDT, OracleDbType.Varchar2);
                db.AddParameter("endDT", endDT, OracleDbType.Varchar2);
                db.AddParameter("eqid", EQ_ID, OracleDbType.Varchar2);
                db.AddParameter("plant", "P500", OracleDbType.Varchar2);

                return db.ExecTable();
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable MachineProductionReportSum(string EQ_ID, DateTime start, DateTime end)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT /*+ INDEX(TB_PR_M_PROD IX_PR_M_PROD_5)*/ ");
                query.AppendLine("TO_DATE(PROD_DATE,'YYYYMMDD') PROD_DATE_S, sum(PROD_QTY) PROD_QTY ");
                query.AppendLine("FROM TB_PR_M_PROD ");
                query.AppendLine("WHERE USE_YN='Y'");
                query.AppendLine("AND EQ_ID=:eqid ");
                query.AppendLine("AND (PROD_DATE BETWEEN :startDT AND :endDT) ");
                query.AppendLine("AND PLANT_ID=:plant ");
                query.AppendLine("AND LOT_ID IS NOT NULL");
                query.AppendLine("group by (PROD_DATE) ");
                query.AppendLine("order by PROD_DATE ");


                DBOra db = new DBOra(query.ToString());
                db.AddParameter("eqid", EQ_ID, OracleDbType.Varchar2);
                db.AddParameter("startDT", start.ToString("yyyyMMdd"), OracleDbType.Varchar2);
                db.AddParameter("endDT", end.ToString("yyyyMMdd"), OracleDbType.Varchar2);
                db.AddParameter("plant", "P500", OracleDbType.Varchar2);

                return db.ExecTable();
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable MachineProductionReportSumHour(string EQ_ID, DateTime date)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT"); 
                query.AppendLine("TO_DATE(SUBSTR(PROD_TIME, 1, 10), 'YYYYMMDDHH24') PROD_DATE_S,");
                query.AppendLine("SUM(PROD_QTY) PROD_QTY");
                query.AppendLine("FROM TB_PR_M_PROD");
                query.AppendLine("WHERE PROD_DATE = :startdt");
                query.AppendLine("AND EQ_ID = :eqid");
                query.AppendLine("AND USE_YN = 'Y'");
               // query.AppendLine("AND TEST_YN = 'N'");
                query.AppendLine("GROUP BY SUBSTR(PROD_TIME, 1, 10)");
                query.AppendLine("ORDER BY PROD_DATE_S");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("startdt", date.ToString("yyyyMMdd"), OracleDbType.Varchar2);
                db.AddParameter("eqid", EQ_ID, OracleDbType.Varchar2);

                return db.ExecTable();
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable GetDashboardStatus(string WC_ID)
        {
            try
            {
                WC_ID = string.IsNullOrEmpty(WC_ID) ? "" : WC_ID.ToUpper();

                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT DISTINCT");
                query.AppendLine("EQ.EQ_ID,");
                query.AppendLine("EQ.EQ_NAME, ");
                query.AppendLine("CODE.NONWRK_CODE,");
                query.AppendLine("CODE.NONWRK_NAME_1033 as NON_NAME, ");
                query.AppendLine("WRK.ITEM_ID,");
                query.AppendLine("EQ.WC_ID,");
                query.AppendLine("to_date(NON.NONWRK_STIME, 'yyyymmddhh24miss') STIME");
                query.AppendLine("FROM TB_EQ_M_EQUIP EQ");
                query.AppendLine("LEFT JOIN TB_CM_M_NONWRK NON ON NON.EQ_ID = EQ.EQ_ID AND NON.NONWRK_ETIME is null");
                query.AppendLine("LEFT JOIN TB_CM_M_NONWRKCODE CODE on CODE.NONWRK_CODE = NON.NONWRK_CODE");
                query.AppendLine("LEFT JOIN TB_PL_M_WRKORD WRK ON WRK.EQ_ID = EQ.EQ_ID AND WRK.USE_YN = 'Y' AND WRK.DEL_FLAG = 'N' AND WO_PROC_STATE = 'S'");
                query.AppendLine("WHERE EQ.EQ_TYPE = 'P'");
                query.AppendLine("AND EQ.USE_YN = 'Y'");
                query.AppendLine("AND EQ.FACT_ID = 'NEX1'");
                query.AppendLine("AND EQ.EQ_ID NOT IN ('10004','10005','10006','10007','10017','10024','10032')");
                if (WC_ID != "")
                    query.AppendLine("AND EQ.WC_ID = :wc");
                query.AppendLine("ORDER BY EQ.EQ_ID");


                DBOra db = new DBOra(query.ToString());
                db.AddParameter("wc", WC_ID, OracleDbType.Varchar2);
                return db.ExecTable();
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable MachineProdActByHour(string EQ_ID)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT SUBSTR(PROD_TIME,9,2) AS HOURPROD,SUM(PROD_QTY) AS PROD");
                query.AppendLine("FROM TB_PR_M_PROD ");
                query.AppendLine("WHERE PROD_DATE = (SELECT TO_CHAR(SYSDATE - 6/24, 'YYYYMMDD') FROM DUAL)");
                query.AppendLine("AND USE_YN='Y'");
                query.AppendLine("AND EQ_ID=:EQID");
                query.AppendLine("GROUP BY SUBSTR(PROD_TIME,9,2)");
                query.AppendLine("ORDER BY HOURPROD");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("EQID", EQ_ID, OracleDbType.Varchar2);
                return db.ExecTable();
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable MachineProdActDay(string EQ_ID)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT SUM(PROD_QTY) AS PROD");
                query.AppendLine("FROM TB_PR_M_PROD ");
                query.AppendLine("WHERE PROD_DATE = (SELECT TO_CHAR(SYSDATE - 6/24, 'YYYYMMDD') FROM DUAL)");
                query.AppendLine("AND USE_YN='Y'");
                query.AppendLine("AND EQ_ID=:EQID");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("EQID", EQ_ID, OracleDbType.Varchar2);
                return db.ExecTable();
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable GetActNonWrk(string EQ_ID)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT /*+INDEX (WRK IX_CM_M_NONWRK_4)*/");
                query.AppendLine("CODE.NONWRK_NAME_1033 AS NON_NAME ");
                query.AppendLine("FROM TB_CM_M_NONWRK WRK ");
                query.AppendLine("LEFT JOIN TB_CM_M_NONWRKCODE CODE ON CODE.NONWRK_CODE = WRK.NONWRK_CODE ");
                query.AppendLine("WHERE WRK.NONWRK_STIME IS NOT NULL ");
                query.AppendLine("AND WRK.NONWRK_ETIME IS NULL ");
                query.AppendLine("AND WRK.NONWRK_DATE IS NOT NULL ");
                query.AppendLine("AND WRK.EQ_ID = :eqid ");
                query.AppendLine("AND WRK.PLANT_ID = 'P500' ");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("eqid", EQ_ID, OracleDbType.Varchar2);
                return db.ExecTable();
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable GetPrototypeBOM(string ITEM_ID, string PROTOTYPE_ID, string PROTOTYPE_VER)
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendLine("SELECT ITEM.ITEM_ID AS ITEM_ID, ITEM.ITEM_NAME AS ITEM_NAME ");
                query.AppendLine("FROM TB_CM_M_PROTOTYPE_BOM BOM ");
                query.AppendLine("JOIN TB_CM_M_ITEM ITEM ON ITEM.ITEM_ID = BOM.CHILD_ITEM_ID ");
                query.AppendLine("WHERE ITEM.USE_YN = 'Y' ");
                query.AppendLine("AND BOM.USE_YN = 'Y' ");
                query.AppendLine("AND BOM.ITEM_ID = :itemid ");
                query.AppendLine("AND BOM.PROTOTYPE_ID = :protoid ");
                query.AppendLine("AND BOM.PROTOTYPE_VER = :protover ");
                query.AppendLine("ORDER BY ITEM.ITEM_ID DESC ");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("itemid", ITEM_ID, OracleDbType.Varchar2);
                db.AddParameter("protoid", PROTOTYPE_ID, OracleDbType.Varchar2);
                db.AddParameter("protover", PROTOTYPE_VER, OracleDbType.Varchar2);
                return db.ExecTable();
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable GetBOM(string ITEM_ID)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT ITEM.ITEM_ID AS ITEM_ID, ITEM.ITEM_NAME AS ITEM_NAME ");
                query.AppendLine("FROM TB_CM_M_BOM BOM ");
                query.AppendLine("JOIN TB_CM_M_ITEM ITEM ON ITEM.ITEM_ID = BOM.CHILD_ITEM_ID ");
                query.AppendLine("WHERE ITEM.USE_YN = 'Y' ");
                query.AppendLine("AND BOM.USE_YN = 'Y' ");
                query.AppendLine("AND BOM.ITEM_ID = :itemid");
                query.AppendLine("ORDER BY ITEM.ITEM_ID DESC ");
                DBOra db = new DBOra(query.ToString());
                db.AddParameter("itemid", ITEM_ID, OracleDbType.Varchar2);
                return db.ExecTable();
            }
            catch(Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable GetInputedMaterial(string EQ_ID)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT IO_POSID, LOT_ID, POS.ITEM_ID as ITEM_ID,ITEM_NAME,CART_ID  ");
                query.AppendLine("FROM TB_EQ_M_EQPOS POS ");
                query.AppendLine("LEFT JOIN TB_CM_M_ITEM ITEM ON ITEM.ITEM_ID=POS.ITEM_ID ");
                query.AppendLine("WHERE POS.EQ_ID=:eqid  ");
                query.AppendLine("AND POS.IO_POSGB='I' ");
                query.AppendLine("AND POS.USE_YN='Y' ");
                query.AppendLine("AND POS.CART_SELECT='Y' ");
                query.AppendLine("ORDER BY IO_POSID");
                DBOra db = new DBOra(query.ToString());
                db.AddParameter("eqid", EQ_ID, OracleDbType.Varchar2);
                return db.ExecTable();
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable GetWorkOrderFromEQ(string EQ_ID)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT ");
                query.AppendLine("WO_NO,  ");
                query.AppendLine("to_date(WO_STIME,'yyyymmddhh24miss') WO_STIME, ");
                query.AppendLine("PROD_TYPE,  ");
                query.AppendLine("WRK.ITEM_ID AS ITEM_ID, ");
                query.AppendLine("ITEM_NAME, WO_QTY, PROD_QTY, UNIT, TEST_YN, PROTOTYPE_ID, PROTOTYPE_VER, FN_CM_GET_ITEM_CHANEL_XCHPF('P500',WRK.ITEM_ID) AS XCHPF ");
                query.AppendLine("FROM TB_PL_M_WRKORD WRK ");
                query.AppendLine("LEFT JOIN TB_CM_M_ITEM ITEM ON ITEM.ITEM_ID=WRK.ITEM_ID ");
                query.AppendLine("WHERE WRK.EQ_ID=:eqid AND WRK.USE_YN='Y' AND WRK.WO_PROC_STATE = 'S' ");
                DBOra db = new DBOra(query.ToString());
                db.AddParameter("eqid", EQ_ID, OracleDbType.Varchar2);
                
                return db.ExecTable();
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable GetProductionMonthDays(int month = 0)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT /*+INDEX(TB_PR_M_PROD IX_PR_M_PROD_5)*/  ");
                query.AppendLine("TO_DATE(PROD_DATE,'YYYYMMDD') AS DATETIME,SUM(PROD_QTY) AS QTY, WC_ID ");
                query.AppendLine("FROM TB_PR_M_PROD  ");
                query.AppendLine("WHERE PLANT_ID='P500' ");
                query.AppendLine("AND WC_ID IN ('T','U') ");
                query.AppendLine("AND USE_YN='Y' ");
                query.AppendLine("AND SHIFT IS NOT NULL ");

                if (month == 0)
                {
                    query.AppendLine("AND PROD_DATE LIKE TO_CHAR(SYSDATE,'YYYYMM') || '%' ");
                }
                else
                {
                    string m = month.ToString().PadLeft(2,'0');
                    query.AppendLine("AND PROD_DATE LIKE TO_CHAR(SYSDATE,'YYYY') || '" + m + "%' ");
                }

                query.AppendLine("GROUP BY PROD_DATE, WC_ID ");
                query.AppendLine("ORDER BY PROD_DATE ");
                DBOra db = new DBOra(query.ToString());
                return db.ExecTable();
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable GetProductionYear(int year = 0)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT /*+INDEX (PROD IX_PR_M_PROD_5)*/");
                query.AppendLine("PROD.PROD_DATE AS DATETIME");
                query.AppendLine(",PROD.WC_ID");
                query.AppendLine(",SUM(PROD.PROD_QTY) AS PROD_QTY");
                query.AppendLine("FROM TB_PR_M_PROD PROD");
                query.AppendLine("WHERE PROD.PLANT_ID = 'P500'");
                query.AppendLine("AND PROD.PROD_DATE LIKE '" + year + "%'");
                query.AppendLine("AND SHIFT IS NOT NULL");
                query.AppendLine("GROUP BY PROD.PROD_DATE,PROD.WC_ID");

                DBOra db = new DBOra(query.ToString());
                return db.ExecTable();
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable GetProductionYearControl(int year = 0)
        {
            try
            {
                // StringBuilder query = new StringBuilder();
                //query.AppendLine("select PROD_DATE, WC_ID, SUM(PROD_QTY) PROD_QTY");
                //query.AppendLine("from TB_PR_M_PROD PROD");
                //query.AppendLine("join TB_EQ_M_EQUIP EQ on EQ.EQ_ID = PROD.EQ_ID");
                //query.AppendLine("where PROD.PROD_DATE like '" + year + "%'");
                //query.AppendLine("group by PROD_DATE, WC_ID");
                //query.AppendLine("order by PROD.PROD_DATE");

                //DBMic db = new DBMic(query.ToString());
                DBMic db = new DBMic("NH_YEAR_PRODUCTION");
                db.AddParameter("Year", year, System.Data.SqlDbType.VarChar );
                //return db.ExecTable();
                return db.ExecProcedure();
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable GetNonWorkSum(string EQ_ID)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT  ");
                query.AppendLine("NON.NTIME as NONWRK_TIME, ");
                query.AppendLine("CODE.NONWRK_NAME_1033 as NONWRK_NAME ");
                query.AppendLine("FROM ( ");
                query.AppendLine("        SELECT  ");
                query.AppendLine("        ROUND(SUM(to_date(NONWRK_ETIME,'YYYYMMDDHH24MISS') - to_date(NONWRK_STIME,'YYYYMMDDHH24MISS'))*24*60,1) AS NTIME,  ");
                query.AppendLine("        NONWRK_CODE ");
                query.AppendLine("        FROM TB_CM_M_NONWRK ");
                query.AppendLine("        WHERE NONWRK_DATE = (SELECT TO_CHAR(SYSDATE - 6/24, 'YYYYMMDD') FROM DUAL) ");
                query.AppendLine("        AND EQ_ID=:eqid ");
                query.AppendLine("        AND NONWRK_ETIME is not null ");
                query.AppendLine("        GROUP BY NONWRK_CODE ");
                query.AppendLine("    ) NON ");
                query.AppendLine("LEFT JOIN TB_CM_M_NONWRKCODE CODE ON CODE.NONWRK_CODE=NON.NONWRK_CODE ");
                DBOra db = new DBOra(query.ToString());
                db.AddParameter("eqid", EQ_ID, OracleDbType.Varchar2);
                return db.ExecTable();
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public DataTable GetMachineList(string EQ_ID = "")
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT EQ_ID, EQ_NAME as Name, WC_ID, PROC_ID ");
                query.AppendLine("FROM TB_EQ_M_EQUIP ");
                query.AppendLine("WHERE EQ_TYPE = 'P' ");
                query.AppendLine("AND USE_YN='Y' ");
                query.AppendLine("AND PLANT_ID='P500' ");
                query.AppendLine("AND FACT_ID='NEX1' ");

                if (!string.IsNullOrEmpty(EQ_ID))
                    query.AppendLine("AND EQ_ID=:eqid ");
                else
                    query.AppendLine("ORDER BY EQ_ID");
                
                DBOra db = new DBOra(query.ToString());

                if (!string.IsNullOrEmpty(EQ_ID))
                    db.AddParameter("eqid", EQ_ID, OracleDbType.Varchar2);

                return db.ExecTable();
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        /// <summary>
        /// Gets HTML layout for ESL
        /// </summary>
        /// <param name="CART_ID"></param>
        /// <returns></returns>
        public DataTable SP_IN_H_PROD_LAYOUT(string CART_ID)
        {
            try
            {
                DBOra db = new DBOra("SP_IN_H_PROD_LAYOUT");
                db.AddParameter("AS_CART_ID", CART_ID, OracleDbType.Varchar2);

                db.AddOutput("RC_TABLE", OracleDbType.RefCursor);
                db.AddOutput("RS_CODE", OracleDbType.Varchar2, 100);
                db.AddOutput("RS_MSG", OracleDbType.Varchar2, 100);
                return db.ExecProcedure();
            } 
            catch(Exception ex)
            {
                return new DataTable();
            }
        }

        /// <summary>
        /// Loads basic info from ESL inerface table
        /// </summary>
        /// <param name="CART_ID"></param>
        /// <returns></returns>
        public DataTable SP_DC_H_PROD_API_ESL(string CART_ID, string LOT_ID)
        {
            try
            {
                DBOra db = new DBOra("SP_DC_H_PROD_API_ESL");
                db.AddParameter("AS_CART_ID", CART_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_LOT_ID", LOT_ID, OracleDbType.Varchar2);

                db.AddOutput("RC_TABLE", OracleDbType.RefCursor);
                db.AddOutput("RS_CODE", OracleDbType.Varchar2, 100);
                db.AddOutput("RS_MSG", OracleDbType.Varchar2, 100);
                return db.ExecProcedure();
            } 
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

    }
}
