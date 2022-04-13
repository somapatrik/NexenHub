using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;

namespace NexenHub.Class
{
    public class DBMic
    {
        public SqlConnection Connection;
        public SqlCommand Command;
        private List<SqlParameter> Parameters = new List<SqlParameter>();

        public string query;

        public DBMic(string query)
        {
            this.Connection = new SqlConnection(GlobalSettings.DatabaseControlling);
            this.query = query;
            this.Command = new SqlCommand(this.query, this.Connection);
        }

        public void AddParameter(string name, object value, System.Data.SqlDbType type)
        {
            SqlParameter parameter = new SqlParameter("@"+name, type);
            parameter.Value = value;
            this.Parameters.Add(parameter);
        }

        public void AddOutput(string name, System.Data.SqlDbType type, int size = 0)
        {
            SqlParameter parameter = new SqlParameter(name, type);
            parameter.Direction = System.Data.ParameterDirection.Output;
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
            using (SqlConnection c = this.Connection)
            {
                c.Open();
                using (SqlCommand cmd = this.Command)
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public SqlDataReader ExecReader()
        {
            SendParameters();
            this.Connection.Open();
            return this.Command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }

        public DataTable ExecTable()
        {
            this.Command = new SqlCommand(this.query, this.Connection);
            SendParameters();
            this.Connection.Open();

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(this.Command);
            da.Fill(dt);
            this.Connection.Close();
            return dt;
        }

        public DataTable ExecProcedure()
        {
            this.Command = new SqlCommand(this.query, this.Connection);
            this.Command.CommandType = CommandType.StoredProcedure;
            SendParameters();
            this.Connection.Open();

            //DataSet ds = new DataSet();
            //SqlDataAdapter da = new SqlDataAdapter(this.Command);
            //da.Fill(ds);
            //this.Connection.Close();
            //return ds.Tables[0];

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(this.Command);
            da.Fill(dt);
            this.Connection.Close();
            return dt;

        }
    }
}
