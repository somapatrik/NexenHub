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

        public DataTable DbList { get; set; }


        private GlobalDatabase dbglobal = new GlobalDatabase();

        public void OnGet()
        {
            DbList = dbglobal.GetMachineList();
        }
    }
}
