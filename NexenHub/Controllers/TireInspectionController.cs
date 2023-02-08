using Microsoft.AspNetCore.Mvc;
using NexenHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NexenHub.Class;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using System.Windows.Markup;

namespace NexenHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TireInspectionController : ControllerBase
    {

        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<TireInspectionController> _logger;
        private GlobalDatabase dbglob = new GlobalDatabase();

        public TireInspectionController(ILogger<TireInspectionController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        [HttpGet("{code}")]
        public ActionResult<TireInspection> Get(string code)
        {
            TireInspection tireInspection;
            tireInspection = new TireInspection(code);
            return tireInspection;
        }

        [HttpGet("version")]
        public ActionResult<DateTime> Get()
        {
            var version = new AppVersionTireInspection();
            return version.VersionDate;
        }

        [HttpPost("defectUpload")]
        public async Task<ActionResult> Post()
        {
            try
            {
                var httpRequest = HttpContext.Request;

                if (httpRequest.Form.Files.Count > 0 && httpRequest.Form.ContainsKey("barcode") && httpRequest.Form.ContainsKey("inspectionResult"))
                {

                    // Barcode
                    string barcode = httpRequest.Form["barcode"].ToString();

                    // Inspection result
                    FertInspectionResult res = JsonConvert.DeserializeObject<FertInspectionResult>(httpRequest.Form["inspectionResult"].ToString());

                    var file = httpRequest.Form.Files[0];

                    // Image root directory
                    var filePath = Path.Combine(_environment.ContentRootPath, "images");

                    // Photo name
                    string originalName = Path.GetFileNameWithoutExtension(file.FileName);
                    string originalExtension = Path.GetExtension(file.FileName);
                    int seq = dbglob.GetBadPhotoSeq(barcode, res.SEQ, res.PROC, res.InspectionTime.ToString("yyyyMMddHHmmss"), res.BAD_ID);

                    string photoName = $"{originalName}_{seq}{originalExtension}";

                    // Save folder
                    string folderPath = Path.Combine(filePath, barcode);
                    Directory.CreateDirectory(folderPath);

                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream); 
                        System.IO.File.WriteAllBytes(Path.Combine(folderPath, photoName), memoryStream.ToArray());
                    }

                    // Save info about photo to db
                    dbglob.SaveInspectionPhotoInfo(barcode, res.SEQ, res.PROC, res.InspectionTime, res.BAD_ID, barcode, photoName, null, seq);

                    return Ok();


                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, "error");
                return new StatusCodeResult(500);
            }

            return BadRequest();
        }

        [HttpPost("defectGallery")]
        public ActionResult<List<string>> GetDefectGallery()
        {
            try
            {
                var httpRequest = HttpContext.Request;

                if (!httpRequest.Form.ContainsKey("barcode") || !httpRequest.Form.ContainsKey("inspectionResult"))
                    return BadRequest();

                // Barcode
                string barcode = httpRequest.Form["barcode"].ToString();

                // Inspection result
                FertInspectionResult inspection = JsonConvert.DeserializeObject<FertInspectionResult>(httpRequest.Form["inspectionResult"].ToString());

                DataTable dt = dbglob.GetDefectGalleryPaths(barcode, inspection.SEQ, inspection.PROC, inspection.InspectionTime, inspection.BAD_ID);
                
                List<string> data = new List<string>();
                
                foreach (DataRow row in dt.Rows)
                    data.Add(Path.Combine(row["SERVER_URL"].ToString(), row["SERVER_DIRECTORY"].ToString(), row["PHOTO_NAME"].ToString()));

                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "error");
                return new StatusCodeResult(500);
            }
        }


        [HttpGet("insProc")]
        public ActionResult<List<ComboItem>> GetInsProc()
        {            
            List<ComboItem> items = new List<ComboItem>();
            DataTable dt = dbglob.CM_CODE_LIST("QA", "01", "02", "P", "1029");
            foreach (DataRow row in dt.Rows)
                items.Add(new ComboItem(row["CODE_ID"].ToString(), row["CODE_NAME"].ToString()));

            return items;
        }

        [HttpGet("defectCode/{Insp}")]
        public ActionResult<List<ComboItem>> GetDefectCode(string Insp)
        {
            List<ComboItem> items = new List<ComboItem>(); ; 

            DataTable dt = dbglob.GetBadTypes(Insp, Language_CD:"1029");

            foreach (DataRow row in dt.Rows)
                items.Add(
                    new ComboItem() { 
                        ID = row["CODE_ID"].ToString(), 
                        Value = row["CODE_NAME"].ToString(), 
                        Value2 = $"[{row["CODE_ID"].ToString()}] {row["CODE_NAME"].ToString()}",
                        }
                    );

            return items;
        }

        [HttpGet("defectGrade/{Insp}/{BadCode}")]
        public ActionResult<List<ComboItem>> GetDefectGrade(string Insp, string BadCode)
        {
            List<ComboItem> items = new List<ComboItem>(); ;

            DataTable dt = dbglob.GetBadGrade(Insp,BadCode);

            if (dt.Rows.Count > 0)
            {
                foreach (DataColumn col in dt.Columns)
                    if (!string.IsNullOrEmpty(dt.Rows[0][col.ColumnName].ToString()))
                        items.Add(new ComboItem()
                        {
                            ID = dt.Rows[0][col.ColumnName].ToString(),
                            Value = dt.Rows[0][col.ColumnName].ToString()
                        });
            }

            return items;
        }

        [HttpGet("originProc")]
        public ActionResult<List<ComboItem>> GetOriginProc()
        {
            List<ComboItem> items = new List<ComboItem>(); ;

            DataTable dt = dbglob.GetCodeDetail("QA","31");

            foreach (DataRow row in dt.Rows)
                items.Add(
                    new ComboItem()
                    {
                        ID = row["CODE_ID"].ToString(),
                        Value = row["CODE_NAME_1033"].ToString(),
                        Value2 = $"[{row["CODE_ID"].ToString()}] {row["CODE_NAME_1033"].ToString()}",
                    }
                    );

            return items;
        }



    }
}
