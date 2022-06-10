using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexenHub.Class;

namespace NexenHub.Pages.Dashboard
{
    public class tvModel : PageModel
    {

        public bool allPingOK;
        public List<PingDevice> PingDevices;
        public List<PingDevice> FailPing;

        public bool isPM;
        public List<StatusEq> MachinesPM;

        private GlobalDatabase dbglob = new GlobalDatabase();

        public void OnGet()
        {
            MachinesPM = new List<StatusEq>();
            foreach (DataRow row in dbglob.GetDashboardStatus("").Rows)
            {
                if (row["NONWRK_CODE"].ToString() == "N016" && row["WC_ID"].ToString() != "U")
                    MachinesPM.Add(new StatusEq() { Name = row["EQ_NAME"].ToString(), Downtime = row["NON_NAME"].ToString() });
            }

            isPM = MachinesPM.Count > 0 ? true : false;


            PingDevices = new List<PingDevice>();
            foreach (DataRow row in dbglob.GetPingDevices().Rows)
                PingDevices.Add(new PingDevice() { Name = row["DISPLAYNAME"].ToString(), IP = row["IP"].ToString() });

            FailPing = PingDevices.FindAll(d => d.PingResult == false);
            allPingOK = FailPing.Count > 0 ? false : true;


        }

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
                    PingReply reply = p.Send(_IP);
                    if (reply.Status == IPStatus.Success)
                        PingResult = true;
                    else
                        PingResult = false;

                }
                catch(Exception ex)
                {
                    PingResult = false;
                }
            }

        }
    }
}
