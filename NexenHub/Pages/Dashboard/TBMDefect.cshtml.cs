using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexenHub.Class;
using NexenHub.Models;

namespace NexenHub.Pages.Dashboard
{
    public class TBMDefectModel : PageModel
    {

        public string Image64 { get; set; }

        private GlobalDatabase dbglob = new GlobalDatabase();
        
        public string Left_EQ_ID { get; set; }
        public string Right_EQ_ID { get; set; }
        public string LeftName { get; set; }
        public string RightName { get; set; }
        public string LeftProd { get; set; }
        public string RightProd { get; set; }

        public WorkorderPlanTBM planLeft { get; set; }

        public WorkorderPlanTBM planRight { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Refresh { get; set; }

        public string DefectHeader { get; set; }

        public void OnGet(int ID)
        {
            switch (ID)
            {
                case 1:
                    Left_EQ_ID = "10033";
                    Right_EQ_ID = "10039";
                    LeftName = "TBM 11";
                    RightName = "TBM 21";
                    break;
                case 2:
                    Left_EQ_ID = "10034";
                    Right_EQ_ID = "10040";
                    LeftName = "TBM 12";
                    RightName = "TBM 22";
                    break;
                case 3:
                    Left_EQ_ID = "10035";
                    Right_EQ_ID = "10041";
                    LeftName = "TBM 13";
                    RightName = "TBM 23";
                    break;
                case 4:
                    Left_EQ_ID = "10036";
                    Right_EQ_ID = "10042";
                    LeftName = "TBM 14";
                    RightName = "TBM 24";
                    break;
                case 5:
                    Left_EQ_ID = "10037";
                    Right_EQ_ID = "10043";
                    LeftName = "TBM 15";
                    RightName = "TBM 25";
                    break;
                case 6:
                    Left_EQ_ID = "10038";
                    Right_EQ_ID = "10044";
                    LeftName = "TBM 16";
                    RightName = "TBM 26";
                    break;

            }

            GetPlan();
            GetProd();
            GetImageInfo();
            //GetImageFromFtp();
        }

        private void GetPlan()
        {
            planLeft = new WorkorderPlanTBM(Left_EQ_ID);
            planRight = new WorkorderPlanTBM(Right_EQ_ID);
            
        }

        private void GetProd()
        {
            LeftProd = dbglob.MachineProdActDay(Left_EQ_ID).Rows[0]["PROD"].ToString();
            RightProd = dbglob.MachineProdActDay(Right_EQ_ID).Rows[0]["PROD"].ToString();
        }

        private void GetImageInfo()
        {
            DataTable dt = dbglob.GetDefectMonitoringPicture();
            if (dt.Rows.Count > 0)
            {
                //DefectHeader = dt.Rows[0]["BAD_ID"].ToString();
                GetImageFromFtp(dt.Rows[0]["IMG_NAME"].ToString());
            }
        }

        private void GetImageFromFtp(string imgname)
        {
            string ftp = "ftp://172.15.0.99:21/";
            string ftpFolder = "EP/DEFECT_MONITORING/";
            try
            {
                //string fileName = "EV-11760-VM-003-000.png";

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftp + ftpFolder + imgname);
               // FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://"+fullpath);
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