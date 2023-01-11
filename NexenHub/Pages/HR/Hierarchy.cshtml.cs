using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static NexenHub.ViewModels.LotParentsViewModel;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;
using Newtonsoft.Json;
using NexenHub.Class;
using System.Linq;

namespace NexenHub.Pages.HR
{
    public class HierarchyModel : PageModel
    {
        public List<NetworkNodeLink> Links { get; set; }
        public List<NetworkNode> Nodes { get; set; }
        public List<NodeData> dbData { get; set; }
        public string FormatNodes => JsonConvert.SerializeObject(Nodes, Formatting.Indented);

        public string FormatLinks => JsonConvert.SerializeObject(Links, Formatting.Indented);


        private GlobalDatabase database = new GlobalDatabase();
        public void OnGet()
        {
            LoadData();
        }

        public void LoadData()
        {
            Links = new List<NetworkNodeLink>();
            Nodes = new List<NetworkNode>();
            dbData = new List<NodeData>();

            // Get links do a list, datatable lacks options
            DataTable dt = database.GetDepartmentRelations();
            foreach (DataRow row in dt.Rows)
                dbData.Add(new NodeData()
                {
                    DEPT_ID = row["DEPT_ID"].ToString(),
                    DEPT_NAME = row["DEPT_NAME"].ToString(),
                    UP_DEPT_ID = row["UP_DEPT_ID"].ToString(),
                    UP_DEPT_NAME = row["UP_DEPT_NAME"].ToString()
                });

            dbData.ForEach(d => Links.Add(new NetworkNodeLink() { from = d.DEPT_ID, to = d.UP_DEPT_ID }));


            // Join all distinct departmens 
            var distinctFrom = Links.Select(x => x.from).Distinct().ToList();
            var distinctTo = Links.Select(x => x.to).Distinct().ToList();
            var distinct = distinctFrom.Union(distinctTo).ToList();

            // Create all nodes
            distinct.ForEach(n =>
            {

                string deptName = dbData.Find(d => d.DEPT_ID == n)?.DEPT_NAME;
                if (deptName == null)
                    deptName = dbData.Find(d => d.UP_DEPT_ID == n).UP_DEPT_NAME;

                Nodes.Add(new NetworkNode() { id = n, label = deptName });
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
            public string DEPT_ID;
            public string DEPT_NAME;
            public string UP_DEPT_ID;
            public string UP_DEPT_NAME;
        }

    }
}
