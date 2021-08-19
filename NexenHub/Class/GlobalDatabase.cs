﻿using System;
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
        public DataTable GetProductionMonthDays()
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
                query.AppendLine("AND PROD_DATE LIKE TO_CHAR(SYSDATE,'YYYYMM') || '%' ");
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
                query.AppendLine("        ROUND(SUM((NONWRK_ETIME - NONWRK_STIME))/60, 1) AS NTIME,  ");
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

        public DataTable GetMachineList()
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
                query.AppendLine("ORDER BY EQ_ID");
                DBOra db = new DBOra(query.ToString());
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
