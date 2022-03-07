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
                query.AppendLine("SELECT");
                query.AppendLine("EQ.EQ_ID,");
                query.AppendLine("EQ.EQ_NAME, ");
                query.AppendLine("CODE.NONWRK_CODE,");
                query.AppendLine("CODE.NONWRK_NAME_1033 as NON_NAME, ");
                //query.AppendLine("NVL(REPLACE(CODE.REL05,'000','0'),'0,0,0') as FRCOLOR,  ");
                //query.AppendLine("NVL(REPLACE(CODE.DISP_COLOR,'000','0'),'255,255,255') as BGCOLOR, ");
                query.AppendLine("WRK.ITEM_ID");
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

        public DataTable MachineProdAct(string EQ_ID)
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

        // TODO: CART_SELECT
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
                query.AppendLine("ORDER BY POS.ITEM_ID DESC ");
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
                query.AppendLine("TO_DATE(WO_STIME,'YYYYMMDDHH24MISS') AS WO_STIME, ");
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
