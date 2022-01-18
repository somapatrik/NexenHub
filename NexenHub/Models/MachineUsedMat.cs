﻿using Newtonsoft.Json;
using NexenHub.Class;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Models
{
    public class MachineUsedMat
    {
        GlobalDatabase dbglob = new GlobalDatabase();

        public string EQ_ID;

        public DateTime StartDate;
        public DateTime EndDate;

        public string UniqeName;

        public DataTable dtUsed;
        public DataTable dtWo;

        public List<visItem> visItems;
        public List<visItem> visBackground;
        public List<visGroup> visGroups;

        public string formatGroups;
        public DateTime startFilterDate;
        public DateTime endFilterDate;

        public MachineUsedMat(string EQ_ID)
        {
            this.EQ_ID = EQ_ID;
            this.StartDate = DateTime.Now;
            this.EndDate = DateTime.Now;

            InitValues();
        }

        public MachineUsedMat(string EQ_ID, DateTime startDT, DateTime endDt)
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
            visItems = new List<visItem>();
            visGroups = new List<visGroup>();
            visBackground = new List<visItem>();

            startFilterDate = StartDate.AddHours(-6);//.ToString("yyyy-MM-ddTHH:mm:ss");
            endFilterDate = EndDate.AddHours(+30);//.ToString("yyyy-MM-ddTHH:mm:ss");

            LoadData();
            ProcessData();
            GenerateName();

        }

        private void LoadData()
        {
            dtUsed = dbglob.MachineReportUsedMat(EQ_ID, StartDate, EndDate);
            dtWo = dbglob.MachineReportWorkOrders(EQ_ID, StartDate, EndDate);
        }

        private void ProcessData()
        {

            int i = 0;
            foreach (DataRow r in dtUsed.Rows)
            {
                string IO = r["IO_POSGB"].ToString();
                string dbGroup = r["IO_POSID"].ToString();
                string dbLOT = r["LOT_ID"].ToString();
                string dbcartid = r["CART_ID"].ToString();
                DateTime date = DateTime.Parse(r["ENT_DT"].ToString());

                if (IO == "I")
                {
                    visItem foundI = visItems.Find(o => o.LOT == dbLOT && o.group == dbGroup && o.start == DateTime.MinValue && o.end >= date);
                    if (foundI != null)
                        foundI.start = date;
                    else 
                        visItems.Add(new visItem() { id=i.ToString(), LOT=dbLOT, start = date, content = dbLOT + " (" + dbcartid + ")", group = dbGroup});
                }

                if (IO == "O")
                {
                    visItem foundO = visItems.Find(o => o.LOT == dbLOT && o.group == dbGroup && o.end == DateTime.MinValue && o.start <= date);
                    if (foundO != null)
                        foundO.end = date;
                    else
                        visItems.Add(new visItem() { id = i.ToString(), LOT = dbLOT, end = date, content = dbLOT + "("+ dbcartid +")", group = dbGroup }); ;
                }

                i++;

                visGroup foundG = visGroups.Find(f => f.id == dbGroup);
                if (foundG == null)
                    visGroups.Add(new visGroup() { id = dbGroup, content = dbGroup });
            }

            // At default datetime to empty dates
            foreach (visItem item in visItems)
            {
                if (item.start == DateTime.MinValue)
                    item.start = startFilterDate;

                if (item.end == DateTime.MinValue)
                    item.end = endFilterDate;

            }


            formatGroups = JsonConvert.SerializeObject(visGroups, Formatting.Indented);

            // Background WO
            foreach (DataRow row in dtWo.Rows)
            {
                visBackground.Add(new visItem()
                {
                    id = i.ToString(),
                    content = "WO: " + row["WO_NO"].ToString(),
                    start = string.IsNullOrEmpty(row["WO_STIME"].ToString()) ? DateTime.MinValue : DateTime.Parse(row["WO_STIME"].ToString()),
                    end = string.IsNullOrEmpty(row["WO_ETIME"].ToString()) ? DateTime.MinValue : DateTime.Parse(row["WO_ETIME"].ToString())
                });
                
                i++;
            }

        }

        private void GenerateName()
        {
            UniqeName = DateTime.Now.ToString("yyyyMMddHHmmss") + EQ_ID;
        }

    }

    public class visItem
    {
        public string id { get; set; }
        public string content { get; set; }
        public DateTime start { set; get; }
        public DateTime end { set; get; }

        public string startS
        {
            get
            {
                return start == DateTime.MinValue ? "" : start.ToString("yyyy-MM-ddTHH:mm:ss");
            }
        }

        public string endS
        {
            get
            {
                return end == DateTime.MinValue ? "" : end.ToString("yyyy-MM-ddTHH:mm:ss");
            }
        }

        public string LOT { get; set; }
        public string group { get; set; }
    }

    public class visGroup
    {
        public string id { get; set; }
        public string content { get; set; }
    }

}
