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
