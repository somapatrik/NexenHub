using Newtonsoft.Json;
using NexenHub.Class;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Models
{
    public class LotParentsViewModel
    {
        public string LOT { get; set; }

        public List<NetworkNodeLink> Links { get; set; }
        public List<NetworkNode> Nodes { get; set; }

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

            // Get links
            DataTable dt = database.GetAllParents(LOT);
            foreach (DataRow row in dt.Rows)
                Links.Add(new NetworkNodeLink() { from = row["INPUT_LOT_ID"].ToString(), to = row["PROD_LOT_ID"].ToString() });

            // Get nodes
            var distinctFrom = Links.Select(x => x.from).Distinct().ToList();
            var distinctTo = Links.Select(x => x.to).Distinct().ToList();
            var distinct = distinctFrom.Union(distinctTo).ToList();
            distinct.ForEach(n => Nodes.Add(new NetworkNode() { id = n, label = n, color = n == LOT ? "#ffc107" : "lightblue" }));
                
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

    }
}
