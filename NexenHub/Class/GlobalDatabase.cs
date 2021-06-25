using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using NexenHub.Class;
using Oracle.ManagedDataAccess.Client;

namespace NexenHub.Class
{
    public class GlobalDatabase
    {

        /// <summary>
        /// Loads basic info from ESL inerface table
        /// </summary>
        /// <param name="CART_ID"></param>
        /// <returns></returns>
        public DataTable SP_DC_H_PROD_API_ESL(string CART_ID)
        {
            try
            {
                DBOra db = new DBOra("SP_DC_H_PROD_API_ESL");
                db.AddParameter("AS_CART_ID", CART_ID, OracleDbType.Varchar2);

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
