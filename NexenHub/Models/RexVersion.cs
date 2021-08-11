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

        DateTime _actversion = new DateTime(2021, 7, 23, 13, 0, 0);
        bool _force = false;

        public DateTime Version
        {
            get
            {
                return _actversion;
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
