using Newtonsoft.Json;
using NexenHub.Class;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.ViewModels
{
    public class LotParentsViewModel
    {
        public string LOT { get; set; }

        public List<NetworkNodeLink> Links { get; set; }
        public List<NetworkNode> Nodes { get; set; }

        public List<NodeData> dbData { get; set; }

        public string FormatNodes => JsonConvert.SerializeObject(Nodes, Formatting.Indented);

        public string FormatLinks => JsonConvert.SerializeObject(Links, Formatting.Indented);


        private GlobalDatabase database = new GlobalDatabase();

        public LotParentsViewModel(string lot)
        {
            LOT = lot;
            LoadData();
        }

        public void LoadData()
        {
            Links = new List<NetworkNodeLink>();
            Nodes = new List<NetworkNode>();
            dbData = new List<NodeData>();

            // Get links do a list, datatable lacks options
            DataTable dt = database.GetAllParentsExperiment(LOT);
            foreach (DataRow row in dt.Rows)
                dbData.Add(new NodeData() 
                { 
                    INPUT_LOT = row["INPUT_LOT_ID"].ToString(), 
                    INPUT_NAME = row["INPUT_NAME"].ToString(),
                    PROD_LOT = row["PROD_LOT_ID"].ToString(),
                    PROD_NAME = row["PROD_NAME"].ToString()
                });
            // Links.Add(new NetworkNodeLink() { from = row["INPUT_LOT_ID"].ToString(), to = row["PROD_LOT_ID"].ToString() });


            dbData.ForEach(d => Links.Add(new NetworkNodeLink() { from = d.INPUT_LOT, to = d.PROD_LOT }));
                

            // Join all distinct lots 
            var distinctFrom = Links.Select(x => x.from).Distinct().ToList();
            var distinctTo = Links.Select(x => x.to).Distinct().ToList();
            var distinct = distinctFrom.Union(distinctTo).ToList();

            // Create all nodes
            distinct.ForEach(n =>
            { 

                string itemName = dbData.Find(d=> d.INPUT_LOT == n)?.INPUT_NAME;
                if (itemName == null)
                    itemName = dbData.Find(d => d.PROD_LOT == n).PROD_NAME;

                Nodes.Add(new NetworkNode() { id = n, label = itemName, color = n == LOT ? "#ffc107" : "lightblue" });
            });
                
        }

        public class NetworkNode
        {
            public string id;
            public string label;
            public string color;
            public string shape = "box";
        }

        public class NetworkNodeLink
        {
            public string from;
            public string to;
            public string arrows = "to";
        }

        public class NodeData
        {
            public string INPUT_LOT;
            public string PROD_LOT;
            public string INPUT_NAME;
            public string PROD_NAME;
        }

    }
}
