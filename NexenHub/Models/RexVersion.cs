using NexenHub.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Models
{
    public class RexVersion
    {

        string _contentType = "application/vnd.android.package-archive";
        string _fileName = "rex.apk";
        string _filepath = "download/com.nexentire.rex.apk";

        //DateTime _actversion = new DateTime(2022, 10, 5, 0, 0, 0);
        AppVersionRex appVersion = new AppVersionRex();

        bool _force = false;

        public DateTime Version
        {
            get
            {
                return appVersion.VersionDate;
            }
        }

        public bool ForceUpdate
        {
            get
            {
                return _force;
            }
        }

        public string GetContentType()
        {
            return _contentType;
        }

        public string GetFileName()
        {
            return _fileName;
        }

        public string GetRelativeFilePath()
        {
            return _filepath;
        }

        
    }
}
