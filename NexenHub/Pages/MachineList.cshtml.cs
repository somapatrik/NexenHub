using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexenHub.Class;

namespace NexenHub.Pages
{
    public class MachineListModel : PageModel
    {
        public List<MachineListObject> MixMachines => DbList.FindAll(x => x.WC_ID == "M");
        public List<MachineListObject> ExtMachines => DbList.FindAll(x => x.WC_ID == "E");
        public List<MachineListObject> CalMachines => DbList.FindAll(x => x.WC_ID == "C");

        public List<String> Procs => DbList.GroupBy(x => x.WC_ID).Select(f=>f.Key).ToList();


        public List<MachineListObject> DbList;

        private GlobalDatabase dbglobal = new GlobalDatabase();


        public void OnGet()
        {
            CreateList();
        }

        public void OnPostFilter(string id)
        {
            CreateList();
            DbList = DbList.FindAll(m => { return m.WC_ID == id; });
        }

        private void CreateList()
        {
            // Make static
            string[] DoNotShow =
            {
                "10004",
                "10005",
                "10006",
                "10007",
                "10017",
                "10024",
                "10032"
            };

            DataTable dt = dbglobal.GetMachineList(); ;
            DbList = new List<MachineListObject>();
            foreach (DataRow r in dt.Rows)
            {
                if (!DoNotShow.Contains(r["EQ_ID"].ToString()))
                    DbList.Add(new MachineListObject
                    {
                        EQ_ID = r["EQ_ID"].ToString(), 
                        Name = r["Name"].ToString(),
                        WC_ID = r["WC_ID"].ToString(),
                        PROC_ID = r["PROC_ID"].ToString()
                    });
            }
        }

    }
}
