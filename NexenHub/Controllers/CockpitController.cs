﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NexenHub.Class;
using NexenHub.Models;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Metadata;

namespace NexenHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CockpitController : ControllerBase
    {

        GlobalDatabase database = new GlobalDatabase();

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

                    if ((Waiting.Count + Started.Count + Finished.Count) < 7)
                    {
                        if (wo.WO_PROC_STATE == "W")
                            Waiting.Add(wo);
                        else if (wo.WO_PROC_STATE == "F")
                            Finished.Add(wo);
                    }

                }

                return new {Waiting, Started,Finished};
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
