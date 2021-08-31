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
        public DataTable DbData { get; set; }
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

            DataTable dt = dbglobal.GetMachineList(); ;
            DbList = new List<MachineListObject>();
            foreach (DataRow r in dt.Rows)
            {
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

    public class MachineListObject
    {
        public string EQ_ID { get; set; }
        public string Name { get; set; }
        public string WC_ID { get; set; }
        public string PROC_ID { get; set; }
    }

}
