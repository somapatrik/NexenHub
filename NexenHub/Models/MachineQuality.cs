using NexenHub.Class;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace NexenHub.Models
{
    public class MachineQuality
    {
        public string EQ_ID { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public List<LotInfo> GoodLots { get; set; }
        public List<LotInfo> BadLots { get; set; }
        public List<LotInfo> ScrapLots { get; set; }

        // Calculations
        public double prodGoodSummary => GoodLots.Sum(g => g.PROD_QTY);
        public double prodBadSummary => BadLots.Sum(g => g.PROD_QTY);
        public double prodScrapSummary => ScrapLots.Sum(g => g.PROD_QTY);
        public double prodSummary => prodGoodSummary + prodBadSummary + prodScrapSummary;

        public double prodCurrentGoodSummary => GoodLots.Sum(g => g.CURRENT_QTY);
        public double prodCurrentBadSummary => BadLots.Sum(g => g.CURRENT_QTY);
        public double prodCurrentScrapSummary => ScrapLots.Sum(g => g.CURRENT_QTY);

        public int lotSummary => GoodLots.Count + BadLots.Count + ScrapLots.Count;

        public double BadPercent => GetPercent(prodBadSummary, prodSummary);
        public double ScrapPercent => GetPercent(prodScrapSummary, prodSummary);

        private DataTable rawData;

        private GlobalDatabase database = new GlobalDatabase();

        public MachineQuality(string eqid, DateTime fromFilter, DateTime toFilter)
        {
            EQ_ID = eqid;
            FromDate = fromFilter;
            ToDate = toFilter;

            LoadFromDb();
        }

        public double GetPercent(double portion, double full)
        {
            return Math.Round((portion / full) * 100, 0);
        }

        private void LoadFromDb()
        {
            rawData = database.GetMachineLots(EQ_ID, FromDate, ToDate);

            GoodLots = new List<LotInfo>();
            BadLots = new List<LotInfo>();
            ScrapLots = new List<LotInfo>();

            foreach (DataRow row in rawData.Rows)
            {

                LotInfo lotInfo = new LotInfo()
                {
                    LOT_ID = row["LOT_ID"].ToString(),
                    ITEM_STATE = row["ITEM_STATE"].ToString(),
                    PROD_QTY = Math.Round(double.Parse(row["PROD_QTY"].ToString()),0),
                    CURRENT_QTY = Math.Round(double.Parse(row["CURRENT_QTY"].ToString()),0)
                };

                if (lotInfo.ITEM_STATE == "N")
                    GoodLots.Add(lotInfo);
                else if (lotInfo.ITEM_STATE == "B")
                    BadLots.Add(lotInfo);
                else if (lotInfo.ITEM_STATE == "S")
                    ScrapLots.Add(lotInfo);
            }

        }



        public class LotInfo
        {
            public string LOT_ID { get; set; }
            public string ITEM_STATE { get; set; }
            public double PROD_QTY { get; set; }
            public double CURRENT_QTY { get; set; }
            public double SUCCESS_RATE { get; set; }
        }




    }
}
