using NexenHub.Class;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NexenHub.Models
{
    public class MachineProduction
    {

        GlobalDatabase glob = new GlobalDatabase();

        public string Name { get; set; }

        public string HoursFormat { get => JsonConvert.SerializeObject(Hours, Formatting.Indented); }
        public string ProductionFormat { get => JsonConvert.SerializeObject(Production, Formatting.Indented); }

        public DataTable RawData { get; set; }

        private List<string> Hours; 
        private List<string> Production;

        public MachineProduction (string EQ_ID)
        {
            Name = EQ_ID;
        }

        public void LoadActProd()
        {
            Hours = new List<string>();
            Production = new List<string>();

            DataTable dt = glob.MachineProdAct(Name);
            RawData = dt;
            foreach (DataRow row in dt.Rows)
            {
                Hours.Add(row["HOURPROD"].ToString());
                Production.Add(row["PROD"].ToString());
            }
        }


    }
}
