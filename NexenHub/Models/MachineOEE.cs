using NexenHub.Class;
using System.Collections.Generic;
using System.Data;
using System;
using System.Linq;

namespace NexenHub.Models
{
    public class MachineOEE
    {
        public MachineBasicInfo Info { get; set; }
        public int SecondsSinceStart { get; set; }
        public int DownTimeSumSeconds { get; set; }
        public double DownTimePercent { get; set; }
        public List<DownTimeDetail> DownTimeGroups { get; set; } 

        private GlobalDatabase database = new GlobalDatabase();

        public MachineOEE(string EQ_ID)
        {
            Info = new MachineBasicInfo(EQ_ID);

            GetCurrentMaxTime();
            LoadDownTimes();
            CountEachPercent();
        }

        private void CountEachPercent()
        {
            DownTimeGroups.ForEach(x => x.Percent = Math.Round((x.Seconds / (double)SecondsSinceStart) * 100, 1));
        }

        private void GetCurrentMaxTime()
        {
            DateTime dayStart = DateTime.Now.AddHours(-6).Date.AddHours(6);
            double seconds = Math.Floor((DateTime.Now - dayStart).TotalSeconds);
            SecondsSinceStart = (int)seconds;
        }

        private void LoadDownTimes()
        {
            DownTimeGroups = new List<DownTimeDetail>();
            int sumMachineDownTime = 0;

            foreach (DataRow row in database.GetDownTimesSimple(Info.EQ_ID).Rows)
            {
                int period = string.IsNullOrEmpty(row["NONWRK_SECONDS"].ToString()) ? 0 : int.Parse(row["NONWRK_SECONDS"].ToString());
                sumMachineDownTime += period;

                string category = GlobalSettings.CodeToGroup(row["NONWRK_CODE"].ToString());

                DownTimeDetail existing = DownTimeGroups.FirstOrDefault(d => d.DownTimeName == category);

                if (existing == null)
                {
                    DownTimeGroups.Add(new DownTimeDetail() { Machine = Info.EQ_ID, DownTimeName = category, Seconds = period });
                }
                else
                {
                    existing.Seconds += period;
                }


            };

            DownTimeSumSeconds = sumMachineDownTime;
            DownTimePercent = Math.Round((DownTimeSumSeconds / (double)SecondsSinceStart) * 100, 1);
            
        }
    }
}
