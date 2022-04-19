using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NexenHub.Class;
using Newtonsoft.Json;

namespace NexenHub.Models
{
    public class YearProd
    {

        #region Properties

        public string jsonDays { get => JsonConvert.SerializeObject(_Days, Formatting.Indented); }

        public string jsonGtProd
        {
            get
            {
                return JsonConvert.SerializeObject(_Gt, Formatting.Indented);
            }
        }

        public string jsonTireProd
        {
            get
            {
                return JsonConvert.SerializeObject(_Tire, Formatting.Indented);
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
        private List<string>[] _Gt { get; set; }
        private List<string>[] _Tire { get; set; }

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
                _Gt = new List<string>[12];
                _Tire = new List<string>[12];

                // Prepares all object with init values
                CreateData();

                // Load from DB and fill objects
                LoadProduction();
            }
        }

        private void CreateData()
        {
            // Prepare days for each month
            for (int month = 1; month <=12; month++)
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
                DateTime prodtime = DateTime.ParseExact(row["PROD_DATE"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);
                //DateTime prodtime = DateTime.ParseExact(row["DATETIME"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);
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
                _Gt[i] = GTProd[i].Select(o => o.Value).ToList();
                _Tire[i] = TireProd[i].Select(o => o.Value).ToList();
            }
        }

    }
}
