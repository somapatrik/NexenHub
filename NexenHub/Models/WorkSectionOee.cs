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

        public string FACT_ID { get; set; }

        // 1 working day
        public int DaySeconds => 86400;

        // Time since the start of the nexen day (previous 6am)
        public int TimeSinceStart { get; set; }

        // Sum of all machines downtimes
        public int SumDowntimeSeconds { get; set; }

        // Collective maximum available time for all machines
        public int AvailableTimeAllMachines => DaySeconds * MachineOee.Count();

        // Collective used time for all machines since start of the day
        public int TimeSinceStartAllMachines => TimeSinceStart * MachineOee.Count();

        // Downtime percent for all machines
        public double DowntimePercentAll { get; set; }

        public List<MachineOEE> MachineOee { get; set; }

        private GlobalDatabase dbglob = new GlobalDatabase();

        public WorkSectionOee(string WC, string FACTORY_ID = "")
        {
            WC_ID = WC.ToUpper();
            FACT_ID = FACTORY_ID.ToUpper();

            GetCurrentMaxTime();
            LoadMachines();
            SumTime();
            GetSumPercentForAllMachines();
        }

        private void GetSumPercentForAllMachines()
        {
            DowntimePercentAll = Math.Round((SumDowntimeSeconds / (double)TimeSinceStartAllMachines) * 100, 0);
        }

        private void GetCurrentMaxTime()
        {
            DateTime dayStart = DateTime.Now.AddHours(-6).Date.AddHours(6);
            double seconds = Math.Floor((DateTime.Now - dayStart).TotalSeconds);
            TimeSinceStart = (int)seconds;
        }

        private void SumTime()
        {
            SumDowntimeSeconds = MachineOee.Sum(x=>x.DownTimeSumSeconds);
        }

        private void LoadMachines()
        {
            MachineOee = new List<MachineOEE>();

            DataTable dt = dbglob.GetMachineList(WC_ID: WC_ID, FACT_ID:FACT_ID);

            foreach (DataRow row in dt.Rows)
                if (!GlobalSettings.OEEIgnoredMachines.Contains(row["EQ_ID"].ToString()))
                {
                    MachineOee.Add(new MachineOEE(row["EQ_ID"].ToString()));
                }
        }


    }
}
