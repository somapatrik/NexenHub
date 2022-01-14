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

        public string UniqeName;

        public MachineProdReport(string EQ_ID)
        {
            this.EQ_ID = EQ_ID;
            this.StartDate = DateTime.Now;
            this.EndDate = DateTime.Now;

            InitValues();
        }

        public MachineProdReport(string EQ_ID, DateTime startDT, DateTime endDt)
        {

            this.EQ_ID = EQ_ID;
            if (startDT <= endDt)
            {
                this.StartDate = startDT;
                this.EndDate = endDt;
            }
            else
            {
                this.EndDate = startDT;
                this.StartDate = endDt;
            }

            InitValues();
        }

        private void InitValues()
        {
            dates = new List<string>();
            counts = new List<string>();
            lsProduction = new List<MachineProdReportItem>();

            LoadData();
            ProcessData();
            GenerateName();

        }

        private void ProcessData()
        {
            foreach (DataRow r in dtProduction.Rows)
            {
                DateTime d = DateTime.Parse(r["PROD_DATE_S"].ToString());
                string date = r["PROD_DATE_S"].ToString();
                string sum = r["PROD_QTY"].ToString();

                dates.Add(date);
                counts.Add(sum);

                lsProduction.Add(new MachineProdReportItem()
                { 
                    day = d, 
                    prodSum = double.Parse(sum)
                });
            }

            datesFormat = JsonConvert.SerializeObject(dates, Formatting.Indented);
            countsFormat = JsonConvert.SerializeObject(counts, Formatting.Indented);
        }

        private void GenerateName()
        {
            UniqeName = DateTime.Now.ToString("yyyyMMddHHmmss") + EQ_ID;
        }

        private void LoadData()
        {
            dtProduction = dbglob.MachineProductionReportSum(EQ_ID, StartDate, EndDate);
        }

    }

    public class MachineProdReportItem
    {
        public DateTime day { get; set; }
        public double prodSum { get; set; }
    }
}
