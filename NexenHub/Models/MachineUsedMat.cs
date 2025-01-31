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

            // Limit range to display
            if (StartDate < EndDate.AddDays(-7))
                StartDate = EndDate.AddDays(-7);

            InitValues();
        }

        private void InitValues()
        {
            visItems = new List<visItem>();
            visGroups = new List<visGroup>();
            visBackground = new List<visItem>();

            // Day starts at 6:00 and ends another day in 6:00
            // StartDate 17.1. 00:00 => 17.1. 06:00
            // StartDate 18.1. 00:00 => 19.1. 06:00
            startFilterDate = StartDate.AddHours(6);
            endFilterDate = EndDate.AddHours(30);

            // Do not filter the future (I DON´T WANT YOUR FUCHA!)
            if (endFilterDate > DateTime.Now)
                endFilterDate = DateTime.Now;

            LoadData();
            ProcessData();
            GenerateName();

        }

        private void LoadData()
        {
            dtUsed = dbglob.MachineReportUsedMat(EQ_ID, startFilterDate, endFilterDate);
            dtWo = dbglob.MachineReportWorkOrders(EQ_ID, startFilterDate, endFilterDate);
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
                string dbitemid = r["ITEM_ID"].ToString();
                string dbitemname = r["ITEM_NAME"].ToString();
                DateTime date = DateTime.Parse(r["ENT_DT"].ToString());

                string body = dbitemname + " - " + dbitemid + " </br> " + "<a href=\"/lot/" + dbLOT + "\" target=\"_blank\">" + dbLOT + "</a> - " +dbcartid;

                if (IO == "I")
                {
                    visItem foundI = visItems.Find(o => o.LOT == dbLOT && o.group == dbGroup && o.start == DateTime.MinValue && o.end >= date);
                    if (foundI != null)
                        foundI.start = date;
                    else 
                        visItems.Add(new visItem() { id=i.ToString(), LOT=dbLOT, start = date, content = body, group = dbGroup});
                }

                if (IO == "O")
                {
                    visItem foundO = visItems.Find(o => o.LOT == dbLOT && o.group == dbGroup && o.end == DateTime.MinValue && o.start <= date);
                    if (foundO != null)
                        foundO.end = date;
                    else
                        visItems.Add(new visItem() { id = i.ToString(), LOT = dbLOT, end = date, content = body, group = dbGroup }); ;
                }

                i++;

                // Material groups
                visGroup foundG = visGroups.Find(f => f.id == dbGroup);
                if (foundG == null)
                    visGroups.Add(new visGroup() { id = dbGroup, content = dbGroup });
            }

            // Material groups simply to JSON
            formatGroups = JsonConvert.SerializeObject(visGroups, Formatting.Indented);

            // At default datetime to empty dates in items
            foreach (visItem item in visItems)
            {
                if (item.start == DateTime.MinValue)
                {
                    
                    item.content += "</br>Missing input time";

                    List<visItem> beforeItems = visItems.FindAll(n => n.group == item.group && n.end < item.end);
                    if (beforeItems.Count>0)
                        item.start = beforeItems.Max(d => d.end);
                    else
                        item.start = startFilterDate;

                }
                    

                if (item.end == DateTime.MinValue)
                {
                    
                    

                    List<visItem> afterItems = visItems.FindAll(n => n.group == item.group && n.start > item.start);
                    if (afterItems.Count > 0)
                    {
                        item.end = afterItems.Min(d => d.start);
                        item.content += "</br>Missing output time";
                    }
                    else
                    {
                        item.end = endFilterDate;
                        item.content += "</br>In use";
                    }
                }
            }

            // Background WO
            foreach (DataRow row in dtWo.Rows)
            {
                string wono = row["WO_NO"].ToString();
                string wostime = row["WO_STIME"].ToString();
                string woetime = row["WO_ETIME"].ToString();
                string woitem = row["ITEM_ID"].ToString();
                string woitemname = row["ITEM_NAME"].ToString();


                string body = "";

                if (string.IsNullOrEmpty(woetime))
                    body = woitemname + " - " + woitem + " WO:" + wono + " (active)";
                else
                    body = woitemname + " - " + woitem + " WO:" + wono;

                visBackground.Add(new visItem()
                {
                    id = i.ToString(),
                    content = body,
                    start = string.IsNullOrEmpty(wostime) ? startFilterDate : DateTime.Parse(wostime),
                    end = string.IsNullOrEmpty(woetime) ? endFilterDate : DateTime.Parse(woetime)
                });
                
                i++;
            }

        }

        private void GenerateName()
        {
            UniqeName = DateTime.Now.ToString("usedHHmmss") + EQ_ID;
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
