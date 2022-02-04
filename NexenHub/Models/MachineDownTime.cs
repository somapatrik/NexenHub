using Newtonsoft.Json;
using NexenHub.Class;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Models
{
    public class MachineDownTime
    {

        GlobalDatabase dbglob = new GlobalDatabase();

        public string EQ_ID;

        public DateTime StartDate;
        public DateTime EndDate;

        public DateTime filterStartDate;
        public DateTime filterEndDate;

        public double totalWorkSeconds;
        public double totalDownTimeSeconds;

        public string formatSumLabels;
        public string formatSumDatas;

        public string formatEaLabels;
        public string formatEaDatas;

        public string UniqeName;

        public DataTable dtDownTimes;

        public List<DownTimeSumItem> sumDownTimes;
   
        public MachineDownTime(string EQ_ID)
        {
            this.EQ_ID = EQ_ID;

            // Input dates must be midnight
            this.StartDate = DateTime.Now.Date;
            this.EndDate = DateTime.Now.Date;

            InitValues();
        }

        public MachineDownTime(string EQ_ID, DateTime startDT, DateTime endDt)
        {

            this.EQ_ID = EQ_ID;

            // Input dates must be midnight
            if (startDT <= endDt)
            {
                this.StartDate = startDT.Date;
                this.EndDate = endDt.Date;
            }
            else
            {
                this.EndDate = startDT.Date;
                this.StartDate = endDt.Date;
            }

            // Limit range to one month
            if (StartDate < EndDate.AddMonths(-1))
                StartDate = EndDate.AddMonths(-1);

            InitValues();
        }

        private void InitValues()
        {
            GenerateName();
            SetDates();
            LoadData();
            
            if (dtDownTimes.Rows.Count > 0) 
            { 
                ProcessData();
                FormatData();
            }
        }

        private void SetDates()
        {
            filterStartDate = StartDate.AddHours(6);
            filterEndDate = EndDate.AddHours(30);

            totalWorkSeconds = (filterEndDate - filterStartDate).TotalSeconds;
        }

        private void LoadData()
        {
            dtDownTimes = dbglob.MachineReportDownTimes(EQ_ID, filterStartDate, filterEndDate);
        }

        private void ProcessData()
        {
            sumDownTimes = new List<DownTimeSumItem>();

            foreach (DataRow row in dtDownTimes.Rows)
            {
                DateTime stime = DateTime.Parse(row["STIME"].ToString());
                DateTime etime = DateTime.Parse(row["ETIME"].ToString());
                double diff = (etime - stime).TotalSeconds;
                string code = row["NONWRK_CODE"].ToString();
                string name = row["NONWRK_NAME"].ToString();

                DownTimeSumItem found = sumDownTimes.Find(d=>d.Code == code);
 
                if (found != null)
                    found.Seconds += diff;
                else
                    sumDownTimes.Add(new DownTimeSumItem() { Code = code, Name = name, Seconds = diff });
            }

            // Total DT seconds
            if (sumDownTimes.Count > 0)
                totalDownTimeSeconds = sumDownTimes.Sum(x => x.Seconds);

        }

        private void FormatData()
        {
            // Total sum chart
            List<string> sumLabels = new List<string>();
            List<string> sumDatas = new List<string>();

            sumLabels.Add("Uptime");
            sumLabels.Add("Downtime");
            sumDatas.Add(totalWorkSeconds.ToString());
            sumDatas.Add(totalDownTimeSeconds.ToString());

            formatSumLabels = JsonConvert.SerializeObject(sumLabels, Formatting.None);
            formatSumDatas = JsonConvert.SerializeObject(sumDatas, Formatting.None);

            // Each dt
            List<string> eaDtLabels = new List<string>();
            List<string> eaDtDatas = new List<string>();

            foreach (DownTimeSumItem downtime in sumDownTimes.OrderBy(x => x.Seconds))
            {
                eaDtLabels.Add(downtime.Name);
                eaDtDatas.Add(downtime.Seconds.ToString());
            }

            formatEaLabels = JsonConvert.SerializeObject(eaDtLabels, Formatting.None);
            formatEaDatas = JsonConvert.SerializeObject(eaDtDatas, Formatting.None);
        }

        private void GenerateName()
        {
            UniqeName = DateTime.Now.ToString("HHmmss") + EQ_ID;
        }


        public class DownTimeSumItem
        {
            public string Code;
            public string Name;
            public double Seconds;
        }

    }

}
