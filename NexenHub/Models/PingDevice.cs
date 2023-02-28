using System.Net.NetworkInformation;

namespace NexenHub.Models
{
    public class PingDevice
    {
        public string Name { get; set; }

        private string _IP;
        public string IP
        {
            get { return _IP; }
            set
            {
                _IP = value;
                Ping();
            }
        }

        public bool PingResult { get; set; }

        public void Ping()
        {
            Ping p = new Ping();
            try
            {
                PingReply reply = p.Send(_IP,750);
                if (reply.Status == IPStatus.Success)
                    PingResult = true;
                else
                    PingResult = false;

            }
            catch
            {
                PingResult = false;
            }
        }

    }
}
