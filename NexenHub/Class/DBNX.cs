namespace NexenHub.Class
{
    /// <summary>
    /// NexenHub MSSQL database
    /// </summary>
    public class DBNX : DBMS
    {
        public DBNX(string query) : base(query, GlobalSettings.DatabaseNXMSQL)
        {
            
        } 
    }
}
