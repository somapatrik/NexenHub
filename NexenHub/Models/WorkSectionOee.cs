using NexenHub.Class;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace NexenHub.Models
{
    public class WorkSectionOee
    {
        public string WC_ID { get; set; }
        public int SumSeconds { get; set; }
        public int MaxSeconds => DaySeconds * Machines.Count();
        public int DaySeconds => 86400;

        public int MaxSecondsNow { get; set; }

        public List<DownTimeDetail> MachineTimes { get; set; }


        private List<MachineBasicInfo> Machines { get; set; }

        private GlobalDatabase dbglob = new GlobalDatabase();


        public WorkSectionOee(string WC)
        {
            WC_ID = WC.ToUpper();

            GetCurrentMaxTime();

            LoadMachines();
            LoadDownTimes();
            SumTime();
        }

        private void GetCurrentMaxTime()
        {
            DateTime dayStart = DateTime.Now.AddHours(-6).Date.AddHours(6);
            double seconds = Math.Floor((DateTime.Now - dayStart).TotalSeconds);
            MaxSecondsNow = (int)seconds;
        }

        private void SumTime()
        {
            SumSeconds = MachineTimes.Sum(x => int.Parse(x.Seconds));
        }

        private void LoadMachines()
        {
            Machines = new List<MachineBasicInfo>();
            foreach (DataRow row in dbglob.GetMachineList(WC_ID: WC_ID).Rows)
                if (!GlobalSettings.IgnoredMachines.Contains(row["EQ_ID"].ToString()))
                    Machines.Add(new MachineBasicInfo(row["EQ_ID"].ToString()));
        }

        private void LoadDownTimes()
        {
            MachineTimes = new List<DownTimeDetail>();

            foreach (var machine in Machines) 
            {
                int sumMachineDownTime = 0;

                foreach (DataRow row in dbglob.GetDownTimesSimple(machine.EQ_ID).Rows) 
                {
                    sumMachineDownTime += string.IsNullOrEmpty(row["NONWRK_SECONDS"].ToString()) ? 0 : int.Parse(row["NONWRK_SECONDS"].ToString());
                };

                double MachinePercent = Math.Round((sumMachineDownTime / (double)MaxSecondsNow) * 100,0);

                MachineTimes.Add(new DownTimeDetail()
                {
                    Machine = machine.EQ_ID,
                    DownTimeName = "Any downtime",
                    Seconds = sumMachineDownTime.ToString(),
                    Percent = MachinePercent.ToString()
                });

            }

        }


        public class DownTimeDetail
        {
            public string Machine { get; set; }
            public string DownTimeName{ get; set; }
            public string Seconds { get; set; }
            public string Percent{ get; set; }
}

    }
}
