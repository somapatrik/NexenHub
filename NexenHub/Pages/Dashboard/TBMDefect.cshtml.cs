using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NexenHub.Pages.Dashboard
{
    public class TBMDefectModel : PageModel
    {

        public string Image64 { get; set; }

        public void OnGet()
        {
            GetImageFromFtp();
        }

        private void GetImageFromFtp()
        {
            string ftp = "ftp://172.15.0.99:21/";
            string ftpFolder = "EP/WORK_GUIDE/";
            try
            {
                string fileName = "EV-11760-VM-003-000.png";

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftp + ftpFolder + fileName);
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                request.Credentials = new NetworkCredential("NXMESEP_FTP", "P500@12345!#");
                request.UsePassive = true;
                request.UseBinary = true;
                request.EnableSsl = false;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                using (MemoryStream stream = new MemoryStream())
                {
                    response.GetResponseStream().CopyTo(stream);
                    string base64String = Convert.ToBase64String(stream.ToArray(), 0, stream.ToArray().Length);
                    Image64 = "data:image/png;base64," + base64String;
                }
            }
            catch (WebException ex)
            {
                throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
            }
        }
    }
}
