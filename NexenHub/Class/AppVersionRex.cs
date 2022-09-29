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
            fileName = "rex.apk";
            filePath = "download/com.nexentire.rex.apk";
            VersionDate = new DateTime(2022, 9, 29, 0, 0, 0);
        }
    }
}
