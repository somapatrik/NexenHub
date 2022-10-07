using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexenHub.Class;
using NexenHub.Models;

namespace NexenHub.Pages
{
    public class RexModel : PageModel
    {             
        //public RexVersion version = new RexVersion();

        public AppVersionRex RexVer = new AppVersionRex();

        public void OnGet()
        {

        }

        public ActionResult OnGetDownload()
        {
            // return File(version.GetRelativeFilePath(), version.GetContentType(), version.GetFileName());
            return File(RexVer.filePath, RexVer.contentType, RexVer.fileName);
            
        }
    }
}
