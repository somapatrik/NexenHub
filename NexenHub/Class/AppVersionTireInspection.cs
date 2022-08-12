using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Class
{
    public class AppVersionTireInspection : AndroidAppVersion
    {
        public AppVersionTireInspection()
        {
            fileName = "TireInspection.apk";
            filePath = "download/com.nexentire.tire_inspection-Signed.apk";
            VersionDate = new DateTime(2022, 08, 12, 0,0,0);
        }
    }
}
