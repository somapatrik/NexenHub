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
        public List<String> Procs => DbList.GroupBy(x => x.WC_ID).Select(f=>f.Key).ToList();

        public List<MachineListObject> DbList;

        private GlobalDatabase dbglobal = new GlobalDatabase();


        public void OnGet()
        {
            CreateList();
        }

        private void CreateList()
        {
            DataTable dt = dbglobal.GetMachineList(); ;
            DbList = new List<MachineListObject>();
            foreach (DataRow r in dt.Rows)
            {
                if (!GlobalSettings.IgnoredMachines.Contains(r["EQ_ID"].ToString()))
                    DbList.Add(new MachineListObject
                    {
                        EQ_ID = r["EQ_ID"].ToString(),
                        FACT_ID = r["FACT_ID"].ToString(),
                        Name = r["Name"].ToString(),
                        WC_ID = r["WC_ID"].ToString(),
                        PROC_ID = r["PROC_ID"].ToString()
                    });
            }
        }

    }
}
