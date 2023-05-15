using NexenHub.Class;
using System;
using System.Collections.Generic;
using System.Data;

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

        private DataTable rawData;

        private GlobalDatabase database = new GlobalDatabase();

        public MachineQuality(string eqid, DateTime fromFilter, DateTime toFilter)
        {
            EQ_ID = eqid;
            FromDate = fromFilter;
            ToDate = toFilter;

            LoadFromDb();
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
