using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NexenHub.Class;
using NexenHub.Models;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;

namespace NexenHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CockpitController : ControllerBase
    {

        GlobalDatabase database = new GlobalDatabase();

        [HttpGet("protbom/{ITEM_ID:maxlength(22)}/{PROTOTYPE_ID}/{PROTOTYPE_VER}")]
        public ActionResult<List<object>> getProBom(string ITEM_ID, string PROTOTYPE_ID, string PROTOTYPE_VER)
        {
            try
            {
                List<object> items = new List<object>();
                foreach (DataRow row in database.GetPrototypeBOM(ITEM_ID, PROTOTYPE_ID, PROTOTYPE_VER).Rows)
                {
                    items.Add(new { ITEM_ID = row["ITEM_ID"].ToString(), ITEM_NAME = row["ITEM_NAME"].ToString() });
                }
                return items;
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("bom/{ITEM_ID:maxlength(22)}")]
        public ActionResult<List<object>> getBom(string ITEM_ID)
        {
            try
            {
                List<object> items = new List<object>();
                foreach(DataRow row in database.GetBOM(ITEM_ID).Rows)
                {
                    items.Add(new { ITEM_ID = row["ITEM_ID"].ToString(), ITEM_NAME = row["ITEM_NAME"].ToString() });
                }
                return items;
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("actWo/{EQ_ID:length(5)}")]
        public ActionResult<WorkOrder> getActWo(string EQ_ID)
        {
            try
            {
                WorkOrder wo = new WorkOrder();
                wo.LoadFromMachine(EQ_ID);
                return wo;
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("latestProd/{EQ_ID:length(5)}")]
        public ActionResult<List<object>> getLatestProd(string EQ_ID)
        {
            try
            {
                List<object> latest = new List<object>();
                DataTable dt = database.LatestProduction(EQ_ID);
                foreach(DataRow row in dt.Rows)
                {
                    latest.Add(new
                    {
                        LOT_ID = row["LOT_ID"].ToString(),
                        PROD_DATE = row["PROD_DATE"].ToString(),
                        ITEM_ID = row["ITEM_ID"].ToString(),
                        ITEM_NAME = row["ITEM_NAME"].ToString(),
                        QTY = row["QTY"].ToString(),
                        SAVE_TYPE = row["SAVE_TYPE"].ToString(),
                        USE_YN = row["USE_YN"].ToString()
                    });

                   
                }

                return latest;
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("inputPositions/{EQ_ID:length(5)}")]
        public ActionResult<List<EQPOS>> getInputPositions(string EQ_ID)
        {
            try 
            { 
                return database.GetInputPositions(EQ_ID);
            }
            catch 
            { 
                return StatusCode(StatusCodes.Status500InternalServerError); 
            }
        }

        [HttpGet("woList/{EQ_ID:length(5)}")]
        public ActionResult<object> getWoList(string EQ_ID)
        {
            try
            {

                List<WorkOrder> Waiting = new List<WorkOrder>();
                List<WorkOrder> Started = new List<WorkOrder>();
                List<WorkOrder> Finished = new List<WorkOrder>();


                foreach (DataRow row in database.LoadWorkOrderList(EQ_ID).Rows)
                {
                    WorkOrder wo = new WorkOrder();
                    wo.WO_NO = row["WO_NO"].ToString();
                    wo.WO_STIME = row["WO_STIME"].ToString();
                    wo.WO_ETIME = row["WO_ETIME"].ToString();
                    wo.PLAN_STIME = row["PLAN_STIME"].ToString();
                    wo.PLAN_ETIME = row["PLAN_ETIME"].ToString();
                    wo.PROD_TYPE = row["PROD_TYPE"].ToString();
                    wo.ITEM_ID = row["ITEM_ID"].ToString();
                    wo.ITEM_NAME = row["ITEM_NAME"].ToString();
                    wo.WO_QTY = row["WO_QTY"].ToString();
                    wo.PROD_QTY = row["PROD_QTY"].ToString();
                    wo.UNIT = row["UNIT"].ToString();
                    wo.TEST_YN = row["TEST_YN"].ToString();
                    wo.PROTOTYPE_ID = row["PROTOTYPE_ID"].ToString();
                    wo.PROTOTYPE_VER = row["PROTOTYPE_VER"].ToString();
                    wo.DEL_FLAG = row["DEL_FLAG"].ToString();
                    wo.WO_PROC_STATE = row["WO_PROC_STATE"].ToString();

                    if (wo.WO_PROC_STATE == "S")
                        Started.Add(wo);
                    else if (wo.WO_PROC_STATE == "W")
                        Waiting.Add(wo);
                    else if (wo.WO_PROC_STATE == "F")
                        Finished.Add(wo);

                }

                // Sort waiting for two upcoming
                int CountLast = 2;
                if (Waiting.Count >= CountLast)
                    Waiting = Waiting.Skip(Waiting.Count - CountLast).Take(CountLast).ToList();

                return new {Waiting, Started,Finished};
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
