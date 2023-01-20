using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using NexenHub.Class;
using NexenHub.Models;
using System.Data;
using System.Reflection.Metadata;
using static NexenHub.Controllers.UtilityController;
using static NexenHub.Pages.RAD.PrototypeProgressModel;

namespace NexenHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RadController : Controller
    {
        GlobalDatabase dbglob = new GlobalDatabase();

        [HttpGet("EmrLocation/{emr}")]
        public ActionResult<SimpleChartObject> GetEmrLocations(string emr)
        {
            string upperEMR = emr.ToUpper();
            SimpleChartObject location = new SimpleChartObject();

            location.ChartTitle = upperEMR;

            foreach (DataRow r in dbglob.GetEMRLocations(upperEMR).Rows)
            {
                location.Values.Add(r["CNT_LOC"].ToString());
                location.Labels.Add(r["WH_ID"].ToString());
            }

            return location;
        }

        [HttpGet("EmrVerDefects/{emr}")]
        public ActionResult<ChartObject> GetEmrVersionDefects(string emr)
        {
            string upperEMR = emr.ToUpper();

            ChartObject defectChart = new ChartObject(upperEMR);
            DataTable dt = dbglob.GetEMRDefects(upperEMR);

            foreach (DataRow r in dt.Rows)
            {
                defectChart.AddLabel(r["BAD_ID"].ToString());
            }

            foreach (DataRow r in dt.Rows)
            {
                defectChart.AddDataSet(r["PROTOTYPE_BOM_VER"].ToString());
                defectChart.AddValue(r["PROTOTYPE_BOM_VER"].ToString(), r["BAD_ID"].ToString(), r["CNT"].ToString());
            }

            return defectChart;
        }

        [HttpGet("EmrDetail/{emr}")]
        public ActionResult<EMR> GetEmrDetail(string emr)
        {
            return new EMR(emr.ToUpper());
        }
    }
}
