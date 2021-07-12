using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NexenHub.Pages
{
    public class RexModel : PageModel
    {
        public void OnGet()
        {

        }

        public ActionResult OnGetDownload()
        {
            var contentType = "application/vnd.android.package-archive";
            var fileName = "rex.apk";
            return File("/lib/download/com.nexentire.rex-Signed.apk", contentType, fileName);
        }
    }
}
