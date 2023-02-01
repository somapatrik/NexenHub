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

        // 1 working day
        public int DaySeconds => 86400;

        // Time since the start of the nexen day (previous 6am)
        public int TimeSinceStart { get; set; }

        // Sum of all machines downtimes
        public int SumSeconds { get; set; }

        // Collective maximum available time for all machines
        public int AvailableTimeAllMachines => DaySeconds * Machines.Count();

        // Collective used time for all machines since start of the day
        public int TimeSinceStartAllMachines => TimeSinceStart * Machines.Count();

        public double PercentAllMachines { get; set; }

        // Each machine detail
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
            GetSumPercentForAllMachines();
        }

        private void GetSumPercentForAllMachines()
        {
            double p = Math.Round((SumSeconds / (double)TimeSinceStartAllMachines) * 100, 0);
            if (p > 100)
                p = 100;
            else if (p < 0)
                p = 0;

            PercentAllMachines = 100 - p;
        }

        private void GetCurrentMaxTime()
        {
            DateTime dayStart = DateTime.Now.AddHours(-6).Date.AddHours(6);
            double seconds = Math.Floor((DateTime.Now - dayStart).TotalSeconds);
            TimeSinceStart = (int)seconds;
        }

        private void SumTime()
        {
            SumSeconds = MachineTimes.Sum(x => int.Parse(x.Seconds));
        }

        private void LoadMachines()
        {
            Machines = new List<MachineBasicInfo>();
            foreach (DataRow row in dbglob.GetMachineList(WC_ID: WC_ID).Rows)
                if (!GlobalSettings.OEEIgnoredMachines.Contains(row["EQ_ID"].ToString()) && row["FACT_ID"].ToString() != "NEX2")
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

                double MachinePercent = Math.Round((sumMachineDownTime / (double)TimeSinceStart) * 100,0);

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
