namespace NexenHub.Class
{
    /// <summary>
    /// Aktion database
    /// </summary>
    public class DBAKT : DBMS
    {
        public DBAKT(string query) : base(query, GlobalSettings.DatabaseAktion) { }
    }
}
