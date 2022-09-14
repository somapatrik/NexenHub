using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexenHub.Class;

namespace NexenHub.Pages
{
    public class TireInspectionModel : PageModel
    {

        public AppVersionTireInspection version = new AppVersionTireInspection();

        public void OnGet()
        {

        }

        public ActionResult OnGetDownload()
        {
            return File(version.filePath, version.contentType, version.fileName);
        }
    }
}
