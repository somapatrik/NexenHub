using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Class
{
    public class AndroidAppVersion
    {

        private string _contentType = "application/vnd.android.package-archive";
        private string _fileName;
        private string _filepath;

        private DateTime _VersionDate;

        public DateTime VersionDate
        {
            get => _VersionDate;
            set => _VersionDate = value;
        }

        /// <summary>
        /// Default contentType for Android .apk file
        /// </summary>
        public string contentType
        {
            get => _contentType;
        }

        /// <summary>
        /// File name after download
        /// </summary>
        public string fileName
        {
            get => _fileName;
            set => _fileName = value;
        }

        /// <summary>
        /// File location on the server
        /// </summary>
        public string filePath
        {
            get => _filepath;
            set => _filepath = value;
        }
    }
}
