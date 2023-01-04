using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NexenHub.Class;
using Newtonsoft.Json;

namespace NexenHub.ViewModels
{
    public class YearProd
    {

        #region Properties

        public string jsonDays { get => JsonConvert.SerializeObject(_Days, Formatting.None); }

        public string jsonGtProd
        {
            get
            {
                //return JsonConvert.SerializeObject(_GtRawData, Formatting.None);
                return JsonConvert.SerializeObject(_GtRawData.Select(x => x.Select(y => y == "0" ? "NaN" : y)), Formatting.None);
            }
        }

        public string jsonTireProd
        {
            get
            {
                //return JsonConvert.SerializeObject(_TireRawData, Formatting.None);
                return JsonConvert.SerializeObject(_TireRawData.Select(x => x.Select(y => y == "0" ? "NaN" : y)), Formatting.None);
            }
        }

        public string jsonGtAvg
        {
            get
            {
                return JsonConvert.SerializeObject(_GtAvg.Select(x => x.ToString(GlobalSettings.CzechNum)), Formatting.None);
            }
        }

        public string jsonGtAvgChart
        {
            get
            {
                return JsonConvert.SerializeObject(_GtAvg.Select(x => x == 0 ? "NaN" : x.ToString()), Formatting.None);
            }
        }

        public string jsonTireAvg
        {
            get
            {
                return JsonConvert.SerializeObject(_TireAvg.Select(x => x.ToString(GlobalSettings.CzechNum)), Formatting.None);
            }
        }

        public string jsonTireAvgChart
        {
            get
            {
                return JsonConvert.SerializeObject(_TireAvg.Select(x => x == 0 ? "NaN" : x.ToString()), Formatting.None);
            }
        }

        public string jsonGtTotal
        {
            get
            {
                return JsonConvert.SerializeObject(_GtTotal.Select(x => x.ToString(GlobalSettings.CzechNum)), Formatting.None);
            }
        }

        public string jsonGtTotalChart
        {
            get
            {
                return JsonConvert.SerializeObject(_GtTotal.Select(x => x == 0 ? "NaN" : x.ToString()), Formatting.None);
            }
        }

        public string jsonTireTotal
        {
            get
            {
                return JsonConvert.SerializeObject(_TireTotal.Select(x => x.ToString(GlobalSettings.CzechNum)), Formatting.None);
            }
        }

        public string jsonTireTotalChart
        {
            get
            {
                return JsonConvert.SerializeObject(_TireTotal.Select(x => x == 0 ? "NaN" : x.ToString()), Formatting.None);
            }
        }

        public int actMonth
        {
            get
            {
                return _Year == DateTime.Now.Year ? DateTime.Now.Month : 1;
            }
        }

        #endregion

        private int _Year;

        // X,Y data from database as a list
        private List<ChartXYDate>[] GTProd { get; set; }
        private List<ChartXYDate>[] TireProd { get; set; }

        // X data - Days arrays
        private List<string>[] _Days { get; set; }

        // Y data - Only production array (no days)
        private List<string>[] _GtRawData { get; set; }
        private List<string>[] _TireRawData { get; set; }

        // AVG
        private double[] _GtAvg { get; set; }
        private double[] _TireAvg { get; set; }

        // TOTAL
        private double[] _GtTotal { get; set; }
        private double[] _TireTotal { get; set; }


        private GlobalDatabase dbglob = new GlobalDatabase();

        public YearProd()
        {
            _Year = DateTime.Now.Year;
            InitData();
        }

        public YearProd(int year)
        {
            _Year = year;
            InitData();
        }

        private void InitData()
        {
            if (_Year > 0)
            {
                TireProd = new List<ChartXYDate>[12];
                GTProd = new List<ChartXYDate>[12];
                _Days = new List<string>[12];
                _GtRawData = new List<string>[12];
                _TireRawData = new List<string>[12];
                _GtAvg = new double[12];
                _GtTotal = new double[12];
                _TireAvg = new double[12];
                _TireTotal = new double[12];

                // Prepares all object with init values
                CreateData();

                // Load from DB and fill objects
                LoadProduction();

                // Gets average
                GetAvg();

                // Gets total
                GetTotal();
            }
        }

        private void CreateData()
        {
            // Prepare days for each month
            for (int month = 1; month <= 12; month++)
            {
                var gtlist = new List<ChartXYDate>();
                var tirelist = new List<ChartXYDate>();

                List<DateTime> daysInMonth = Enumerable.Range(1, DateTime.DaysInMonth(_Year, month))  // Days: 1, 2 ... 31
                    .Select(day => new DateTime(_Year, month, day)) // day to a date
                    .ToList();

                foreach (DateTime day in daysInMonth)
                {
                    gtlist.Add(new ChartXYDate() { Date = day, Value = "0" });
                    tirelist.Add(new ChartXYDate() { Date = day, Value = "0" });
                }

                // Order production by date
                gtlist.OrderBy(o => o.Date);
                tirelist.OrderBy(o => o.Date);

                GTProd[month - 1] = gtlist;
                TireProd[month - 1] = tirelist;

                _Days[month - 1] = daysInMonth.Select(day => day.Day.ToString()).ToList();
            }
        }

        private void LoadProduction()
        {
            // Load production
            DataTable dt = dbglob.GetProductionYearControl(_Year);
            //DataTable dt = dbglob.GetProductionYear(_Year);
            foreach (DataRow row in dt.Rows)
            {
                DateTime prodtime = DateTime.ParseExact(row["PROD_DATE"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture); // from controlling
                //DateTime prodtime = DateTime.ParseExact(row["DATETIME"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture); // from mes
                string qty = row["PROD_QTY"].ToString();
                string wc = row["WC_ID"].ToString();

                if (wc == "T")
                    GTProd[prodtime.Month - 1].Find(g => g.Date == prodtime).Value = qty;
                else if (wc == "U")
                    TireProd[prodtime.Month - 1].Find(o => o.Date == prodtime).Value = qty;

            }

            // Copy production from object to json objects
            for (int i = 0; i <= 11; i++)
            {
                _GtRawData[i] = GTProd[i].Select(o => o.Value).ToList();
                _TireRawData[i] = TireProd[i].Select(o => o.Value).ToList();
            }
        }

        private void GetAvg()
        {
            for (int i = 0; i < 12; i++)
            {
                var found = _GtRawData[i].FindAll(gi => double.Parse(gi) > 0);
                if (found.Count > 0)
                    _GtAvg[i] = Math.Round(found.Average(gj => double.Parse(gj)), 0);

                found = _TireRawData[i].FindAll(gi => double.Parse(gi) > 0);
                if (found.Count > 0)
                    _TireAvg[i] = Math.Round(found.Average(gj => double.Parse(gj)), 0);
            }
        }

        private void GetTotal()
        {
            for (int i = 0; i < 12; i++)
            {
                _GtTotal[i] = _GtRawData[i].Select(g => double.Parse(g)).Sum();
                _TireTotal[i] = _TireRawData[i].Select(t => double.Parse(t)).Sum();
            }
        }

    }
}
