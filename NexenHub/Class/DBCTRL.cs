namespace NexenHub.Class
{
    /// <summary>
    /// Controlling database
    /// </summary>
    public class DBCTRL : DBMS
    {
        public DBCTRL(string query) : base(query, GlobalSettings.DatabaseControlling) { }
    }
}
