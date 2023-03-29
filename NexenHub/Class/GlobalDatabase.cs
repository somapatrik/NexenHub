using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.ConstrainedExecution;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NetBarcode;
using NexenHub.Class;
using NexenHub.Models;
using Oracle.ManagedDataAccess.Client;

namespace NexenHub.Class
{
    public class GlobalDatabase
    {
        public DataTable GetOldVersions()
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendLine("SELECT");
                query.AppendLine("'IO Server' SOFTWARE, ");
                query.AppendLine("NMP.NETBIOS_NAME DEVICE, ");
                query.AppendLine("NVL(VER.IP,NMP.IP_ADDRESS) IP");
                query.AppendLine("FROM  TB_CM_M_NMP_SETTINGS NMP");
                query.AppendLine("LEFT JOIN TB_CM_M_VERSION VER on VER.IP=NMP.IP_ADDRESS");
                query.AppendLine("WHERE NMP.USE_YN = 'Y'");
                query.AppendLine("AND (SOFTWARE_ID='ioserver' OR (VER.VERSION_NAME is null AND SOFTWARE_ID is null))");
                query.AppendLine("AND (VER.VERSION_NAME <> (SELECT MAX(VERSION_NAME) FROM TB_CM_M_VERSION WHERE SOFTWARE_ID='ioserver') OR VER.VERSION_NAME is null)");

                query.AppendLine("UNION ALL");

                query.AppendLine("SELECT ");
                query.AppendLine("'ICS' SOFTWARE, ");
                query.AppendLine("ICS.DISPLAYNAME DEVICE,");
                query.AppendLine("NVL(VER.IP,ICS.IP) IP");
                query.AppendLine("FROM TB_CM_M_MONITORING_CONFIG ICS ");
                query.AppendLine("LEFT JOIN TB_CM_M_VERSION VER on VER.IP=ICS.IP");
                query.AppendLine("WHERE ICS.USE_YN = 'Y'");
                query.AppendLine("AND (SOFTWARE_ID='ics' OR (SOFTWARE_ID is null and VERSION_NAME is null))");
                query.AppendLine("AND (VER.VERSION_NAME <> (SELECT MAX(VERSION_NAME) FROM TB_CM_M_VERSION WHERE SOFTWARE_ID='ics') OR VERSION_NAME is null)");

                DBOra db = new DBOra(query.ToString());
                return db.ExecTable();

            }
            catch
            {
                return new DataTable();
            }
        }

        public void TireToRAD(string barcode, string Whcode, string user_id,string InspType)
        {
            try
            {
                DBOra db = new DBOra("SP_IN_MP_FM054");
                db.AddParameter("AS_PLANT_ID", GlobalSettings.PLANT_ID,OracleDbType.Varchar2);
                db.AddParameter("AS_BARCODE_NO", barcode, OracleDbType.Varchar2);
                db.AddParameter("AS_TO_WHCODE", Whcode, OracleDbType.Varchar2);
                db.AddParameter("AS_USER_ID", user_id, OracleDbType.Varchar2);
                db.AddParameter("AS_INSP_TYPE", InspType, OracleDbType.Varchar2);
                db.AddParameter("AS_LANGUAGE_CD", "1029", OracleDbType.Varchar2);

                db.AddOutput("RS_CODE", OracleDbType.Varchar2, 100);
                db.AddOutput("RS_MSG", OracleDbType.Varchar2, 100);

                db.ExecProcedure();
            }
            catch { }
            
        }

        public bool CreateDefect(TireDefect defect, string userID)
        {
            try
            {
                TireInspection tireInfo = new TireInspection(defect.Barcode);

                DBOra db = new DBOra("SP_QA_MP_FM015_INSERT");
                //db.ForceTestDb();

                db.AddParameter("AS_PLANT_ID", GlobalSettings.PLANT_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_FACT_ID", tireInfo.TireProduction.FACT_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_INSP_PROC_ID", defect.INSP, OracleDbType.Varchar2);
                db.AddParameter("AS_BARCODE_NO", tireInfo.Barcode, OracleDbType.Varchar2);
                db.AddParameter("AS_LOT_ID", tireInfo.TireProduction.LOT_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_ITEM_ID", tireInfo.TireProduction.ITEM_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_BAD_ID", defect.BAD_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_BAD_GRADE", defect.BAD_GRADE, OracleDbType.Varchar2);
                db.AddParameter("AS_LOC_MOLD", defect.MOLD, OracleDbType.Varchar2);
                db.AddParameter("AS_LOC_SIDE", defect.SIDE, OracleDbType.Varchar2);
                db.AddParameter("AS_LOC_ZONE", defect.ZONE, OracleDbType.Varchar2);
                db.AddParameter("AS_LOC_POSITION", defect.POS, OracleDbType.Varchar2);
                db.AddParameter("AS_CQ2", defect.CQ2, OracleDbType.Varchar2);
                db.AddParameter("AS_SUNG_EQ_ID", tireInfo.GtProduction.EQ_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_GARYU_EQ_ID", tireInfo.TireProduction.EQ_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_REMARKS", defect.Remark, OracleDbType.Varchar2);
                db.AddParameter("AS_USER_ID", userID, OracleDbType.Varchar2);
                db.AddParameter("AS_CAUSEPROC", defect.CAUSE_PROC, OracleDbType.Varchar2);
                db.AddParameter("AS_LANGUAGE_CD", "1029", OracleDbType.Varchar2);

                db.AddOutput("RS_CODE", OracleDbType.Varchar2, 100);
                db.AddOutput("RS_MSG", OracleDbType.Varchar2, 100);

                db.ExecProcedure();

            }
            catch 
            {
                return false;
            }

            return true;
        }

        public DataTable GetCodeDetail(string SYSCODE, string KINDCODE)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("select CODE_ID, CODE_NAME_1033,CODE_NAME_1029");
                query.AppendLine("from TB_CM_D_CODE");
                query.AppendLine("where SYS_CODE_ID = :syscode");
                query.AppendLine("and KIND_CODE_ID = :kindcode");
                query.AppendLine("and USE_YN = 'Y'");
                query.AppendLine("ORDER BY SORT01 ");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("syscode", SYSCODE, OracleDbType.Varchar2);
                db.AddParameter("kindcode", KINDCODE, OracleDbType.Varchar2);

                return db.ExecTable();
            }
            catch
            {
                return new DataTable();
            }
        }

        public DataTable GetBadGrade(string InsProc, string BadCode)
        {
            try
            {
                //DBOra db = new DBOra("SP_QA_MR_PROC_GRADE");
                //db.AddParameter("AS_PLANT_ID", GlobalSettings.PLANT_ID, OracleDbType.Varchar2);
                //db.AddParameter("AS_PROC_ID", InsProc, OracleDbType.Varchar2);
                //db.AddParameter("AS_BAD_ID", BadCode, OracleDbType.Varchar2);\

                //db.AddOutput("RC_TABLE", OracleDbType.RefCursor);
                //db.AddOutput("RS_CODE", OracleDbType.Varchar2, 100);
                //db.AddOutput("RS_MSG", OracleDbType.Varchar2, 100);

                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT");
                query.AppendLine("DECODE(GRADE_E, 'Y', 'E', null) GRADE_E, ");
                query.AppendLine("DECODE(GRADE_R1, 'Y', 'R1', null) GRADE_R1, ");
                query.AppendLine("DECODE(GRADE_R3, 'Y', 'R3', null) GRADE_R3,");
                query.AppendLine("DECODE(GRADE_K, 'Y', 'K', null) GRADE_K,");
                query.AppendLine("DECODE(GRADE_H, 'Y', 'H', null) GRADE_H,");
                query.AppendLine("DECODE(GRADE_M, 'Y', 'M', null) GRADE_M");
                query.AppendLine("FROM   TB_QA_M_BAD_TYPE");
                query.AppendLine("WHERE  PLANT_ID = :plantid");
                query.AppendLine("AND PROC_ID = :procid");
                query.AppendLine("AND BAD_ID = :badid");
                query.AppendLine("AND USE_YN = 'Y'");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("plantid", GlobalSettings.PLANT_ID, OracleDbType.Varchar2);
                db.AddParameter("procid", InsProc, OracleDbType.Varchar2);
                db.AddParameter("badid",BadCode, OracleDbType.Varchar2);

                return db.ExecTable();
            }
            catch
            {
                return new DataTable();
            }
        }

        public DataTable GetBadTypes(string InsProc = "", string CodeID = "", string CodeName = "", string Language_CD = "1033")
        {
            try
            {
                DBOra db = new DBOra("SP_CM_MR_POP_009");
                db.AddParameter("AS_PLANT_ID", GlobalSettings.PLANT_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_PROC_ID", InsProc, OracleDbType.Varchar2);
                db.AddParameter("AS_CODE_ID", CodeID, OracleDbType.Varchar2);
                db.AddParameter("AS_CODE_NAME", CodeName, OracleDbType.Varchar2);
                db.AddParameter("AS_LANGUAGE_CD", Language_CD, OracleDbType.Varchar2);

                db.AddOutput("RC_TABLE", OracleDbType.RefCursor);
                db.AddOutput("RS_CODE", OracleDbType.Varchar2, 100);
                db.AddOutput("RS_MSG", OracleDbType.Varchar2, 100);

                return db.ExecProcedure();
            }
            catch
            {
                return new DataTable();
            }
        }

        public DataTable CM_CODE_LIST(string SYS_CODE, string KIND_CODE, string REL_NO, string REL_VAL, string Language_CD = "1033")
        {
            try
            {
                DBOra db = new DBOra("SP_CM_MR_CODELIST");
                db.AddParameter("AS_PLANT_ID", GlobalSettings.PLANT_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_SYS_CODE_ID", SYS_CODE, OracleDbType.Varchar2);
                db.AddParameter("AS_KIND_CODE_ID", KIND_CODE, OracleDbType.Varchar2);
                db.AddParameter("AS_REL_NO", REL_NO, OracleDbType.Varchar2);
                db.AddParameter("AS_REL_VAL", REL_VAL, OracleDbType.Varchar2);
                db.AddParameter("AS_LANGUAGE_CD", Language_CD, OracleDbType.Varchar2);

                db.AddOutput("RC_TABLE", OracleDbType.RefCursor);
                db.AddOutput("RS_CODE", OracleDbType.Varchar2, 100);
                db.AddOutput("RS_MSG", OracleDbType.Varchar2, 100);

                return db.ExecProcedure();
            }
            catch 
            {
                return new DataTable();
            }
        }

        #region User

        public string GetNameUser(string id)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT USER_NAME");
                query.AppendLine("FROM TB_CM_M_USER");
                query.AppendLine("WHERE USER_ID = :id");
                query.AppendLine("AND USE_YN = 'Y'");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("id", id, OracleDbType.Varchar2);
                DataTable dt = db.ExecTable();
                if (dt.Rows.Count > 0)
                    return dt.Rows[0][0].ToString();
            }
            catch
            {

            }

            return "";
        }

        #endregion

        #region Login

        public User Login(string ID, string password)
        {
            User user = new User();

            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT USER_ID, USER_NAME");
                query.AppendLine("FROM TB_CM_M_USER");
                query.AppendLine("WHERE USER_ID = :id");
                query.AppendLine("AND SUBSTR(CAL_SHA256(:password),1,64) = SUBSTR(PWD, 1, 64)");
                query.AppendLine("AND USE_YN = 'Y'");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("id", ID, OracleDbType.Varchar2);
                db.AddParameter("password", password, OracleDbType.Varchar2);
                DataTable dt = db.ExecTable();

                if (dt.Rows.Count > 0)
                {
                    user.UserId = dt.Rows[0]["USER_ID"].ToString();
                    user.Name = dt.Rows[0]["USER_NAME"].ToString();
                }
            }
            catch
            { }

            return user;
        }

        public User CardLogin(string hexCardId)
        {
            User user = new User();

            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT US.USER_ID, US.USER_NAME, CARD.CARD_ID_HEX, US.USE_YN");
                query.AppendLine("FROM TB_CM_M_USER US");
                query.AppendLine("JOIN TB_CM_M_MEMBERS_CARD CARD ON CARD.MEMBER_ID=US.USER_ID");
                query.AppendLine("WHERE US.USE_YN='Y'");
                query.AppendLine("AND CARD.USE_YN='Y'");
                query.AppendLine("AND CARD.CARD_ID_HEX = :hex");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("hex", hexCardId, OracleDbType.Varchar2);
                DataTable dt = db.ExecTable();
                if (dt.Rows.Count > 0)
                    user = new User()
                    {
                        UserId = dt.Rows[0]["USER_ID"].ToString(),
                        Name = dt.Rows[0]["USER_NAME"].ToString(),
                        CardHex = hexCardId
                    };

            }
            catch { }

            return user;
        }

        #endregion

        public DataTable GetDepartments(string rootDepartment)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("select DEPT_ID, DEPT_NAME, UP_DEPT_ID, LEVEL");
                query.AppendLine("from TB_CM_M_DEPT");
                query.AppendLine("CONNECT BY PRIOR DEPT_ID = UP_DEPT_ID");
                query.AppendLine("START WITH DEPT_ID = :startdp");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("startdp", rootDepartment, OracleDbType.Varchar2);
                return db.ExecTable();

            }
            catch
            {
                return new DataTable();
            }
        }

        public DataTable GetDownTimesSimple(string EQ_ID)
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendLine("select ");
                query.AppendLine("ROUND(SUM(");
                query.AppendLine("    NVL(TO_DATE(NONWRK_ETIME, 'YYYYMMDDHH24MISS'), SYSDATE)");
                query.AppendLine("    - (SELECT TO_DATE(TO_CHAR(SYSDATE - interval '6' HOUR, 'YYYYMMDD') || '060000', 'YYYYMMDDHH24MISS') FROM dual)");
                query.AppendLine("    ) * 24 * 60 * 60, 0) NONWRK_SECONDS");
                query.AppendLine(", NONWRK_CODE");
                query.AppendLine("from TB_CM_M_NONWRK");
                query.AppendLine("WHERE EQ_ID = :eqid");
                query.AppendLine("AND NONWRK_DATE<TO_CHAR(SYSDATE,'YYYYMMDD')");
                query.AppendLine("AND(NONWRK_ETIME IS NULL OR NONWRK_ETIME > (select to_char(sysdate - interval '6' hour, 'YYYYMMDD') || '060000' from dual))");
                query.AppendLine("GROUP BY NONWRK_CODE");

                query.AppendLine("UNION");

                query.AppendLine("SELECT ");
                query.AppendLine("ROUND(SUM(NVL(TO_DATE(NONWRK_ETIME,'YYYYMMDDHH24MISS'),SYSDATE) - TO_DATE(NONWRK_STIME,'YYYYMMDDHH24MISS'))*24*60*60,0) NONWRK_SECONDS ");
                query.AppendLine(",NONWRK_CODE ");
                query.AppendLine("FROM TB_CM_M_NONWRK ");
                query.AppendLine("WHERE EQ_ID = :eqid ");
                query.AppendLine("AND NONWRK_DATE = (select to_char(sysdate - interval '6' hour, 'YYYYMMDD') from dual) ");
                query.AppendLine("GROUP BY NONWRK_CODE");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("eqid", EQ_ID, OracleDbType.Varchar2);

                return db.ExecTable();
            }
            catch
            {
                return new DataTable();
            }
        }

        #region Prototype progress
        public DataTable GetEMRDefects(string EMR)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT PROD.PROTOTYPE_ID,PROD.PROTOTYPE_BOM_VER, INSP.BAD_ID, COUNT(*) CNT");
                query.AppendLine("FROM TB_QA_H_PROC_BAD_DETAIL INSP");
                query.AppendLine("JOIN TB_PR_M_PROD PROD ON PROD.BARCODE_NO = INSP.BARCODE_NO");
                query.AppendLine("WHERE PROD.PROTOTYPE_ID = :emr");
                query.AppendLine("AND PROD.WC_ID IN('T', 'U')");
                query.AppendLine("AND PROD.USE_YN = 'Y'");
                query.AppendLine("AND INSP.BAD_ID is not null");
                query.AppendLine("GROUP BY PROD.PROTOTYPE_ID, PROD.PROTOTYPE_BOM_VER, INSP.BAD_ID");
                query.AppendLine("ORDER BY PROD.PROTOTYPE_BOM_VER");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("emr", EMR, OracleDbType.Varchar2);

                return db.ExecTable();

            }
            catch
            {
                return new DataTable();

            }
        }

        public DataTable GetEMRLocations(string EMR)
        {
            try 
            { 
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT ");
                query.AppendLine("    WHLOC WH_ID, ");
                query.AppendLine("    COUNT(WHLOC) CNT_LOC");
                query.AppendLine("FROM(");
                query.AppendLine("    SELECT ");
                query.AppendLine("        FN_IN_GET_LAST_LOCATION('P500', PROD.BARCODE_NO) AS WHLOC");
                query.AppendLine("    FROM TB_PR_M_PROD PROD");
                query.AppendLine("    WHERE PROD.PROTOTYPE_ID = :EMR");
                query.AppendLine("    AND PROD.USE_YN = 'Y'");
                query.AppendLine("    AND PROD.WC_ID in ('T','U')");
                query.AppendLine(")");
                query.AppendLine("WHERE WHLOC IS NOT NULL");
                query.AppendLine("GROUP BY WHLOC");
                query.AppendLine("ORDER BY WHLOC");


                //query.AppendLine("SELECT WHLOC WH_ID, COUNT(WHLOC) CNT_LOC");
                //query.AppendLine("FROM(");
                //query.AppendLine("SELECT FN_IN_GET_LAST_LOCATION('P500', PROD.BARCODE_NO) AS WHLOC");
                //query.AppendLine("FROM TB_IN_M_LOT LOT");
                //query.AppendLine("JOIN TB_PR_M_PROD PROD ON PROD.LOT_ID = LOT.LOT_ID");
                //query.AppendLine("JOIN TB_IN_M_LOC LOC ON LOC.LOC_ID = LOT.LOC_NO");
                //query.AppendLine("WHERE PROD.PROTOTYPE_ID = :EMR");
                //query.AppendLine("AND PROD.USE_YN = 'Y'");
                //query.AppendLine("AND LOT.USE_YN = 'Y'");
                //query.AppendLine("AND PROD.WC_ID in ('T', 'U')");
                //query.AppendLine(")");
                //query.AppendLine("WHERE WHLOC IS NOT NULL");
                //query.AppendLine("GROUP BY WHLOC");
                //query.AppendLine("ORDER BY WHLOC");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("EMR", EMR, OracleDbType.Varchar2);

                return db.ExecTable();

            }
            catch
            {
                return new DataTable();

            }

        }
  
        public DataTable GetPrototypeProgressChart(DateTime From , DateTime To, string EMR, string ITEM_ID, string ITEM_NAME, string TEST_TYPE)
        {
            try
            {
                string FromFormat = From.ToString("yyyyMMdd");
                string ToFormat = To.ToString("yyyyMMdd");

               // DBOra db = new DBOra(query.ToString());
                DBOra db = new DBOra("SP_PL_REQ_PROTOTYPE_PROG");

                db.AddParameter("AS_EMR", EMR, OracleDbType.Varchar2);
                db.AddParameter("AS_ITEM_ID", ITEM_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_ITEM_NAME", ITEM_NAME, OracleDbType.Varchar2);
                db.AddParameter("AS_FROM", FromFormat, OracleDbType.Varchar2);
                db.AddParameter("AS_TO", ToFormat, OracleDbType.Varchar2);
                db.AddParameter("AS_TEST_TYPE", TEST_TYPE, OracleDbType.Varchar2);

                db.AddOutput("RC_TABLE", OracleDbType.RefCursor);
                db.AddOutput("RS_CODE", OracleDbType.Varchar2, 100);
                db.AddOutput("RS_MSG", OracleDbType.Varchar2, 100);

                return db.ExecProcedure();
            }
            catch
            {
                return new DataTable();
            }

            
        }
        #endregion

        #region Production plan
        public List<int> GetTBMMonthPlan()
        {

            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT DD01,DD02,DD03,DD04,DD05,DD06,DD07,DD08,DD09,DD10,DD11,DD12,DD13,DD14,DD15,");
            query.AppendLine("DD16,DD17,DD18,DD19,DD20,DD21,DD22,DD23,DD24,DD25,DD26,DD27,DD28,DD29,DD30,DD31");
            query.AppendLine("FROM TB_PL_M_KPI_GOAL_DAY ");
            query.AppendLine("WHERE GOAL_MONTH=TO_CHAR(SYSDATE,'MM') ");
            query.AppendLine("AND GOAL_YEAR = TO_CHAR(SYSDATE,'YYYY')");
            query.AppendLine("AND MNG_TYPE='LOG'");
            query.AppendLine("AND MNG_GOAL = 'DT3'");

            DBOra db = new DBOra(query.ToString());
            DataTable dt = db.ExecTable();
            List<string> days = new List<string>();

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                foreach (DataColumn col in dt.Columns)
                    days.Add(row[col].ToString());
            }

            List<int> numbers = days.Select(x => string.IsNullOrEmpty(x) ? 0 : int.Parse(x)).ToList();

            return numbers;
        }

        public List<int> GetCUREMonthPlan()
        {

            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT DD01,DD02,DD03,DD04,DD05,DD06,DD07,DD08,DD09,DD10,DD11,DD12,DD13,DD14,DD15,");
            query.AppendLine("DD16,DD17,DD18,DD19,DD20,DD21,DD22,DD23,DD24,DD25,DD26,DD27,DD28,DD29,DD30,DD31");
            query.AppendLine("FROM TB_PL_M_KPI_GOAL_DAY ");
            query.AppendLine("WHERE GOAL_MONTH=TO_CHAR(SYSDATE,'MM') ");
            query.AppendLine("AND GOAL_YEAR = TO_CHAR(SYSDATE,'YYYY')");
            query.AppendLine("AND MNG_TYPE='LOG'");
            query.AppendLine("AND MNG_GOAL = 'DU3'");

            DBOra db = new DBOra(query.ToString());
            DataTable dt = db.ExecTable();
            List<string> days = new List<string>();

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                foreach (DataColumn col in dt.Columns)
                    days.Add(row[col].ToString());
            }

            List<int> numbers = days.Select(x => string.IsNullOrEmpty(x) ? 0 : int.Parse(x)).ToList();

            return numbers;
        }

        #endregion

        public DataTable GetDefectGalleryPaths(string AS_BARCODE_NO, int AS_INSP_SEQ, string AS_PROC_ID, DateTime AS_INSP_DT, string AS_BAD_ID)
        {
            try 
            { 
                DBOra db = new DBOra("SP_PROC_BAD_PHOTO_GALLERY");
                db.AddParameter("AS_BARCODE_NO", AS_BARCODE_NO, OracleDbType.Varchar2);
                db.AddParameter("AS_INSP_SEQ", AS_INSP_SEQ, OracleDbType.Int16);
                db.AddParameter("AS_PROC_ID", AS_PROC_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_INSP_DT", AS_INSP_DT.ToString("yyyyMMddHHmmss"), OracleDbType.Varchar2);
                db.AddParameter("AS_BAD_ID", AS_BAD_ID, OracleDbType.Varchar2);
                db.AddOutput("RC_TABLE", OracleDbType.RefCursor);
                return db.ExecProcedure();
            }
            catch
            {
                return new DataTable();
            }
        }
        
        public void SaveInspectionPhotoInfo(string AS_BARCODE_NO,int AS_INSP_SEQ,string AS_PROC_ID,DateTime AS_INSP_DT,string AS_BAD_ID,string AS_DIR,string AS_PHOTO_NAME,string AS_NOTE,int AS_PHOTO_SEQ)
        {
            try
            {
                DBOra db = new DBOra("SP_PROC_BAD_PHOTO_NEW");
                db.AddParameter("AS_BARCODE_NO", AS_BARCODE_NO, OracleDbType.Varchar2);
                db.AddParameter("AS_INSP_SEQ", AS_INSP_SEQ, OracleDbType.Int16);
                db.AddParameter("AS_PROC_ID", AS_PROC_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_INSP_DT", AS_INSP_DT.ToString("yyyyMMddHHmmss"), OracleDbType.Varchar2);
                db.AddParameter("AS_BAD_ID", AS_BAD_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_DIR", AS_DIR, OracleDbType.Varchar2);
                db.AddParameter("AS_PHOTO_NAME", AS_PHOTO_NAME, OracleDbType.Varchar2);
                db.AddParameter("AS_NOTE", AS_NOTE, OracleDbType.Varchar2);
                db.AddParameter("AS_PHOTO_SEQ", AS_PHOTO_SEQ, OracleDbType.Int16);
                db.ExecProcedure();
            }
            catch
            {

            }
        }
        
        public int GetBadPhotoSeq(string AS_BARCODE_NO,int AS_INSP_SEQ,string AS_PROC_ID,string AS_INSP_DT,string AS_BAD_ID)
        {
            try 
            { 
                DBOra db = new DBOra("select FN_QA_DEFECT_PHOTO_SEQ(:AS_BARCODE_NO,:AS_INSP_SEQ,:AS_PROC_ID,:AS_INSP_DT,:AS_BAD_ID) FROM DUAL");
                db.AddParameter("AS_BARCODE_NO", AS_BARCODE_NO, OracleDbType.Varchar2);
                db.AddParameter("AS_INSP_SEQ", AS_INSP_SEQ, OracleDbType.Int16); 
                db.AddParameter("AS_PROC_ID", AS_PROC_ID, OracleDbType.Varchar2);
                db.AddParameter("AS_INSP_DT", AS_INSP_DT, OracleDbType.Varchar2);
                db.AddParameter("AS_BAD_ID", AS_BAD_ID, OracleDbType.Varchar2);
                DataTable dt = db.ExecTable();
                return int.Parse(dt.Rows[0][0].ToString());
            }
            catch
            {
                return 0;
            }

}

        public DataTable GetAllParentsExperiment(string LOT)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT");
                query.AppendLine("TRC.PROD_LOT_ID, ");
                query.AppendLine("TRC.INPUT_LOT_ID, ");
                query.AppendLine("TRC.EVENT_TIME,");
                query.AppendLine("TRC.PROD_ITEM_ID, ");
                query.AppendLine("TRC.INPUT_ITEM_ID, ");
                query.AppendLine("TRIM(REPLACE(ITEMPROD.ITEM_NAME,'Compound','')) PROD_NAME,");
                query.AppendLine("TRIM(REPLACE(ITEMINPUT.ITEM_NAME,'Compound','')) INPUT_NAME");


                query.AppendLine("FROM(");
                query.AppendLine("SELECT PROD_LOT_ID, INPUT_LOT_ID, to_date(EVENT_TIME, 'YYMMDDHH24MISS') EVENT_TIME, ITEM_ID PROD_ITEM_ID, CHILD_ITEM_ID INPUT_ITEM_ID");
                query.AppendLine("FROM TB_IN_M_ITEM_TRACE TRC");
                query.AppendLine("CONNECT BY PRIOR INPUT_LOT_ID = PROD_LOT_ID");
                query.AppendLine("START WITH PROD_LOT_ID = :lot");
                query.AppendLine("GROUP BY PROD_LOT_ID, INPUT_LOT_ID, EVENT_TIME, ITEM_ID, CHILD_ITEM_ID");
                query.AppendLine("ORDER BY EVENT_TIME, PROD_LOT_ID");
                query.AppendLine(") TRC");
                query.AppendLine("LEFT JOIN TB_CM_M_ITEM ITEMINPUT ON ITEMINPUT.ITEM_ID = TRC.INPUT_ITEM_ID");
                query.AppendLine("LEFT JOIN TB_CM_M_ITEM ITEMPROD ON ITEMPROD.ITEM_ID = TRC.PROD_ITEM_ID");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("lot", LOT, OracleDbType.Varchar2);

                DataTable dt = db.ExecTable();
                return dt;
            }
            catch 
            {
                return new DataTable();
            }
        }

        public DataTable GetLotParents(string LOT)
        {
            try
            {
                DBOra db = new DBOra("select INPUT_LOT_ID from TB_IN_M_ITEM_TRACE where PROD_LOT_ID = :lot");
                db.AddParameter("lot", LOT, OracleDbType.Varchar2);

                DataTable dt = db.ExecTable();
                return dt;
            }
            catch 
            {
                return new DataTable();
            }
        }

        public void UpdateVersion(string appID, string IP, string VersionName)
        {
            try
            {
                DBOra db = new DBOra("SP_CM_M_VERSION_UPDATE");
                db.AddParameter("AS_IP", IP, OracleDbType.Varchar2);
                db.AddParameter("AS_SOFTWARE_ID", appID, OracleDbType.Varchar2);
                db.AddParameter("AS_VERSION_NAME", VersionName, OracleDbType.Varchar2);

               // db.AddOutput("RC_TABLE", OracleDbType.RefCursor);
                db.AddOutput("RS_CODE", OracleDbType.Varchar2, 100);
                db.AddOutput("RS_MSG", OracleDbType.Varchar2, 100);

                db.ExecProcedure();

            }
            catch 
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
            catch 
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
            catch 
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
            catch 
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
            catch 
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
            catch
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
            catch 
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
            catch 
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
            catch 
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
                query.AppendLine("PROC_ID,");
                query.AppendLine("FN_CM_GET_CODE_NAME('QA', '01', PROC_ID, '1033') AS PROC_ID_FULLNAME,");
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
            catch 
            {
                return new DataTable();
            }
}

        public string Cart2Lot(string CART_ID)
        {
            try
            {
                DBOra db = new DBOra("SELECT FN_IN_CART_TO_LOT(:cart) FROM DUAL ");
                db.AddParameter("cart", CART_ID.ToUpper(), OracleDbType.Varchar2);
                DataTable dt = db.ExecTable();
                return dt.Rows.Count > 0 ? dt.Rows[0][0].ToString() : "";
            }
            catch
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
            catch
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
            catch
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
            catch
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
            catch 
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
            catch
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
            catch 
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
            catch 
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
            catch 
            {
                return new DataTable();
            }
        }

        public DataTable MembersByDepartment(string deptID)
        {
            try
            {

                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT MEM.MEMBER_ID, MEM.DEPT_ID, MEM.MEMBER_NAME, LOWER(MEM.EMAIL_ADDR) EMAIL, MEM.TTOUT1 POSITION");
                query.AppendLine("FROM TB_CM_M_MEMBER MEM");
                query.AppendLine("where MEM.USE_YN = 'Y'");
                query.AppendLine("and MEM.EMP_STATS = 1");
                query.AppendLine("and MEM.DEPT_ID=:deptid");
                query.AppendLine("order by ZTITEL");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("deptid", deptID, OracleDbType.Varchar2);
                DataTable dt = db.ExecTable();
                return dt;

            }
            catch
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

                DBAKT db = new DBAKT(query.ToString());
                db.AddParameter("memberid", member_id, SqlDbType.VarChar);
                DataTable dt = db.ExecTable();
                return dt;

            }
            catch 
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
            catch 
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
            catch 
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
            catch 
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
            catch 
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
            catch 
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
            catch 
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
            catch 
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
            catch 
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
            catch 
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
            catch 
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
            catch 
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
                query.AppendLine("HIS.LOC_NO LOC_NO,");
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
            catch 
            {
                return new DataTable();
            }
        }

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
            catch 
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
            catch 
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
            catch
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
            catch 
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
            catch
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
            catch 
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
            catch 
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
            catch 
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
            catch 
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
            catch 
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
            catch 
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
            catch 
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
            catch 
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
            catch
            {
                return new DataTable();
            }
        }

        public List<EQPOS> GetInputPositions(string EQ_ID)
        {
            List<EQPOS> positions = new List<EQPOS>();

            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendLine("SELECT ");
                query.AppendLine("POS.EQ_ID EQ_ID,");
                query.AppendLine("POS.IO_POSID IO_POSID,"); 
                query.AppendLine("POS.LOT_ID LOT_ID,");
                query.AppendLine("POS.CART_ID CART_ID,");
                query.AppendLine("POS.IO_POSGB IO_POSGB,");
                query.AppendLine("POS.ITEM_ID ITEM_ID,");
                query.AppendLine("POS.USE_YN USE_YN,");
                query.AppendLine("READER.SYNC_ID SYNC_ID");
                query.AppendLine("FROM TB_EQ_M_EQPOS POS");
                query.AppendLine("LEFT JOIN TB_CM_M_MONITORING_READER READER ON READER.EQ_ID=POS.EQ_ID AND READER.POSID=POS.IO_POSID");
                query.AppendLine("WHERE POS.EQ_ID=:eqid");

                DBOra db = new DBOra(query.ToString());
                db.AddParameter("eqid", EQ_ID);
                foreach(DataRow row in db.ExecTable().Rows)
                {
                    positions.Add(new EQPOS() 
                    {
                        EQ_ID = row["EQ_ID"].ToString(),
                        IO_POSID = row["IO_POSID"].ToString(),
                        SYNC_ID = row["SYNC_ID"].ToString(),
                        LOT_ID = row["LOT_ID"].ToString(),
                        IO_POSGB = row["IO_POSGB"].ToString(),
                        CART_ID = row["CART_ID"].ToString(),
                        ITEM_ID = row["ITEM_ID"].ToString(),
                        USE = row["USE_YN"].ToString() == "Y"
                    });
                }
            }
            catch{ }

            return positions;
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
            catch 
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
            catch 
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
            catch 
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
            catch 
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
                DBCTRL db = new DBCTRL("NH_YEAR_PRODUCTION");
                db.AddParameter("Year", year, System.Data.SqlDbType.VarChar );
                //return db.ExecTable();
                return db.ExecProcedure();
            }
            catch 
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
            catch
            {
                return new DataTable();
            }
        }

        public DataTable GetMachineList(string EQ_ID = "", string WC_ID = "", string FACT_ID = "")
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT EQ_ID, EQ_NAME as Name, WC_ID, PROC_ID, FACT_ID ");
                query.AppendLine("FROM TB_EQ_M_EQUIP ");
                query.AppendLine("WHERE EQ_TYPE = 'P' ");
                query.AppendLine("AND USE_YN='Y' ");
                query.AppendLine("AND PLANT_ID='P500' ");

                if (string.IsNullOrEmpty(FACT_ID))
                    query.AppendLine("AND FACT_ID IN ('NEX1','NEX2') ");
                else
                    query.AppendLine("AND FACT_ID = :factid ");

                if (!string.IsNullOrEmpty(WC_ID))
                    query.AppendLine("AND WC_ID = :wcid ");

                if (!string.IsNullOrEmpty(EQ_ID))
                    query.AppendLine("AND EQ_ID = :eqid ");
                else
                    query.AppendLine("ORDER BY EQ_ID");
                
                DBOra db = new DBOra(query.ToString());

                if (!string.IsNullOrEmpty(FACT_ID))
                    db.AddParameter("factid", FACT_ID, OracleDbType.Varchar2);

                if (!string.IsNullOrEmpty(WC_ID))
                    db.AddParameter("wcid", WC_ID, OracleDbType.Varchar2);

                if (!string.IsNullOrEmpty(EQ_ID))
                    db.AddParameter("eqid", EQ_ID, OracleDbType.Varchar2);

                return db.ExecTable();
            }
            catch
            {
                return new DataTable();
            }
        }

    }
}
