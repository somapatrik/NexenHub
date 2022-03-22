using Newtonsoft.Json;
using NexenHub.Class;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Models
{
    /*
     * This class will return machine production data for
     * selected dates. It will also store graph data
     */

    public class MachineProdReport
    {
        GlobalDatabase dbglob = new GlobalDatabase();

        public string EQ_ID;

        public DateTime StartDate;
        public DateTime EndDate;

        public DataTable dtProduction;
        public List<MachineProdReportItem> lsProduction;

        public List<string> dates;
        public List<string> counts;

        public string datesFormat;
        public string countsFormat;

        public string maxValue
        {
            get { return lsProduction.Count > 0 ? maxVal.ToString(GlobalSettings.CzechNum) : ""; }
        }
        public string minValue
        {
            get { return lsProduction.Count > 0 ? minVal.ToString(GlobalSettings.CzechNum) : ""; }
        }
        public string avgValue
        {
            get { return lsProduction.Count > 0 ? avgVal.ToString(GlobalSettings.CzechNum) : ""; }
        }
        public string sumValue
        {
            get { return lsProduction.Count > 0 ? sumVal.ToString(GlobalSettings.CzechNum) : ""; }
        }

        private double maxVal;
        private double minVal;
        private double avgVal;
        private double sumVal;

        public string UniqeName;

        public MachineProdReport(string EQ_ID)
        {
            this.EQ_ID = EQ_ID;

            // Input dates must be midnight
            this.StartDate = DateTime.Now.Date;
            this.EndDate = DateTime.Now.Date;

            InitValues();
        }

        public MachineProdReport(string EQ_ID, DateTime startDT, DateTime endDt)
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

            // Limit range to one year
            if (StartDate < EndDate.AddYears(-1))
                StartDate = EndDate.AddYears(-1);

            InitValues();
        }

        private void InitValues()
        {
            dates = new List<string>();
            counts = new List<string>();
            lsProduction = new List<MachineProdReportItem>();

            FillDatesX();

            LoadData();
            ProcessData();
            GenerateName();

        }

        // Will prepare X axis with dates / times
        private void FillDatesX()
        {
            if (StartDate == EndDate)
            {
                DateTime fillStart = StartDate.AddHours(6); // 00:00 + 06:00
                DateTime fillEnd = fillStart.AddHours(23);  // 06:00 -> 05:00

                if (fillEnd > DateTime.Now)
                    fillEnd = DateTime.Now;

                // Fill hour by hour
                while (fillStart <= fillEnd)
                {
                    lsProduction.Add(new MachineProdReportItem() { day = fillStart, prodSum = 0 });
                    dates.Add(fillStart.ToString("yyyy-MM-dd HH-mm-ss"));
                    fillStart = fillStart.AddHours(1);
                }
            }
            else
            {
                DateTime fill = StartDate;

                // Fill day by day
                while (fill <= EndDate)
                {
                    lsProduction.Add(new MachineProdReportItem() { day = fill, prodSum = 0 });
                    dates.Add(fill.ToString("yyyy-MM-dd"));
                    fill = fill.AddDays(1);

                }
            }
        }

        private void ProcessData()
        {
            foreach (DataRow r in dtProduction.Rows)
            {
                DateTime d = DateTime.Parse(r["PROD_DATE_S"].ToString());
                string sum = r["PROD_QTY"].ToString();

                MachineProdReportItem found = lsProduction.Find(o => o.day == d);
                if (found != null)
                    found.prodSum = double.Parse(sum);
               
            }

            if (lsProduction.Count > 0)
            {
                maxVal = lsProduction.Max(m => m.prodSum);

                var foundMin = lsProduction.FindAll(o => o.prodSum > 0);
                if (foundMin.Count > 0)
                    minVal = lsProduction.FindAll(o => o.prodSum > 0).Min(mi => mi.prodSum);
                else
                    minVal = 0;
                
                avgVal = lsProduction.Average(a => a.prodSum);
                sumVal = lsProduction.Sum(a => a.prodSum);
            }

            counts = lsProduction.Select(o => o.prodSum.ToString()).ToList();

            datesFormat = JsonConvert.SerializeObject(dates, Formatting.Indented);
            countsFormat = JsonConvert.SerializeObject(counts, Formatting.Indented);
        }

        private void GenerateName()
        {
            UniqeName = DateTime.Now.ToString("yyyyMMddHHmmss") + EQ_ID;
        }

        private void LoadData()
        {
            if (StartDate == EndDate)
                dtProduction = dbglob.MachineProductionReportSumHour(EQ_ID, StartDate);
            else
                dtProduction = dbglob.MachineProductionReportSum(EQ_ID, StartDate, EndDate);
        }

    }

    public class MachineProdReportItem
    {
        public DateTime day { get; set; }
        public double prodSum { get; set; }
    }
}
