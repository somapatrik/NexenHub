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
        public async Task<ActionResult<TireInspection>> Get(string code)
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
                    
                    // Get photo SEQ
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
                _logger.LogError(e, "error");
                return new StatusCodeResult(500);
            }

            return BadRequest();
        }
    }
}
