using Microsoft.Data.SqlClient;
using static NexenHub.Class.DBMic;

namespace NexenHub.Class
{
    public class DBNX : DBMS
    {
        public DBNX(string query) : base(query, GlobalSettings.DatabaseNXMSQL)
        {
            
        } 
    }
}
