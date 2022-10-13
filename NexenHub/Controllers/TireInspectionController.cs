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

                if (httpRequest.Form.Files.Count > 0)
                {

                    foreach (var file in httpRequest.Form.Files)
                    {
                        var filePath = Path.Combine(_environment.ContentRootPath, "images");

                        if (!Directory.Exists(filePath))
                            Directory.CreateDirectory(filePath);

                        using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream); System.IO.File.WriteAllBytes(Path.Combine(filePath, file.FileName), memoryStream.ToArray());
                        }

                        return Ok();
                    }
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
