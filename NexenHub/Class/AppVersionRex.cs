using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Class
{
    public class AppVersionRex : AndroidAppVersion
    {
        public AppVersionRex()
        {
            VersionDate = new DateTime(2023, 3, 14, 0, 0, 0);
            filePath = "download/com.nexentire.rex.apk";
            fileName = "rex_" + VersionDate.ToString("yyMMdd") + ".apk";
        }
    }
}
