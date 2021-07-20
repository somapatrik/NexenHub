using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexenHub.Models;

namespace NexenHub.Pages
{
    public class RexModel : PageModel
    {             
        public RexVersion version = new RexVersion();

        public void OnGet()
        {

        }

        public ActionResult OnGetDownload()
        {
            
            return File(version.GetRelativeFilePath(), version.GetContentType(), version.GetFileName());
        }
    }
}
