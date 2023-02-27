using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Class
{
    public class DBOra
    {
        public OracleConnection Connection;
        public OracleCommand Command;
        private List<OracleParameter> Parameters = new List<OracleParameter>();

        public string query;

        public DBOra(string query)
        {
            this.Connection = GlobalSettings.Test ? new OracleConnection(GlobalSettings.DatabaseConnectionDevelopment) : new OracleConnection(GlobalSettings.DatabaseConnection);
            this.query = query;
            this.Command = new OracleCommand(this.query, this.Connection);
        }

        public void ForceTestDb()
        {
            Connection = new OracleConnection(GlobalSettings.DatabaseConnectionDevelopment);
            Command = new OracleCommand(this.query, this.Connection);
        }

        public void AddParameter(string name, object value, OracleDbType type)
        {
            OracleParameter parameter = new OracleParameter(name, type);
            parameter.Value = value;
            this.Parameters.Add(parameter);
        }

        public void AddOutput(string name, OracleDbType type, int size = 0)
        {
            OracleParameter parameter = new OracleParameter(name, type);
            parameter.Direction = ParameterDirection.Output;
            parameter.Value = DBNull.Value;
            if (size > 0)
                parameter.Size = size;

            this.Parameters.Add(parameter);
        }

        public void SendParameters()
        {
            if (this.Parameters.Count > 0)
                this.Command.Parameters.AddRange(this.Parameters.ToArray());
        }

        public void Exec()
        {
            SendParameters();
            using (OracleConnection c = this.Connection)
            {
                c.Open();
                using (OracleCommand cmd = this.Command)
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public OracleDataReader ExecReader()
        {
            SendParameters();
            this.Connection.Open();
            return this.Command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public DataTable ExecTable()
        {
            //this.Command = new OracleCommand(this.query, this.Connection);
            //SendParameters();
            //this.Connection.Open();

            //DataTable dt = new DataTable();
            //OracleDataAdapter da = new OracleDataAdapter(this.Command);
            //da.Fill(dt);
            //this.Connection.Close();

            DataTable dt = new DataTable();
            SendParameters();
            using (OracleConnection c = this.Connection)
            {
                c.Open();
                using (OracleCommand cmd = this.Command)
                {
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    da.Fill(dt);
                }
            }

            return dt;
        }

        public DataTable ExecProcedure()
        {
            this.Command = new OracleCommand(this.query, this.Connection);
            this.Command.CommandType = CommandType.StoredProcedure;
            SendParameters();
            this.Connection.Open();

            DataSet ds = new DataSet();
            OracleDataAdapter da = new OracleDataAdapter(this.Command);
            da.Fill(ds);
            this.Connection.Close();

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return new DataTable();

        }
    }
}
