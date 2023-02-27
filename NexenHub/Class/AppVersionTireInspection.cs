﻿using System;
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
            filePath = "download/com.nexentire.tire_inspection.apk";
            VersionDate = new DateTime(2023,2,23,0,0,0);
        }
    }
}
