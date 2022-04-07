using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using NexenHub.Models;

namespace NexenHub.Class
{
    public class InputCheck
    {
        private string _LOT_ID;
        private string _EQ_ID;
        private string _MINIPC_ID;
        public string LOT_ID
        {
            get { return _LOT_ID; }
            set
            {
                _LOT_ID = value;
                LoadLotCheck();
            }
        }

        public bool ExpireCheckResult;
        public bool AgingCheckResult;
        public bool FIFOCheckResult;
        public bool NoValidCheckResult;
        public bool NoLifeCheckResult;
        public bool PremiumGtResult;
        public bool IsGtDOnlyResult;
        public bool AllCheck;

        public string Expire { get => ExpireCheckResult.ToString(); }
        public string Aging { get => AgingCheckResult.ToString(); }
        public string FIFO { get => FIFOCheckResult.ToString(); }
        public string Specification { get => NoValidCheckResult.ToString(); }
        public string Enabled { get => NoLifeCheckResult.ToString(); }
        public string Premium { get => PremiumGtResult.ToString(); }
        public string GtDOnly { get => IsGtDOnlyResult.ToString(); }
        public string IsValidMaterial { get => AllCheck.ToString(); }

        private WorkOrder _workOrder = new WorkOrder();
        private LotItem _lotInfo;
        private GlobalDatabase dbglob = new GlobalDatabase();

        public InputCheck(string LOTID)
        {
            _LOT_ID = LOTID;
            LoadLotCheck();
        }

        // For test
        public InputCheck(string LOTID, string EQID, string MiniPcID)
        {
            _LOT_ID = LOTID;
            _EQ_ID = EQID;
            _MINIPC_ID = MiniPcID;
            LoadLotCheck();
        }

        private void LoadLotCheck()
        {
            _lotInfo = new LotItem(_LOT_ID);
            if (_lotInfo.Valid)
                Check();
        }

        private void Check()
        {
            try
            {
                ExpireCheckResult = false;
                AgingCheckResult = false;
                FIFOCheckResult = false;
                NoValidCheckResult = false;
                NoLifeCheckResult = false;
                PremiumGtResult = false;
                AllCheck = false;

                _workOrder.LoadLikeICS(_EQ_ID, _MINIPC_ID);
                if (!_workOrder.WO_EXISTS)
                    return;

                Task expireTask = Task.Run(() => { ExpireCheck(); });
                Task agingTask = Task.Run(() => { AgingCheck(); });
                Task fifoTask = Task.Run(() => { FIFOCheck(); });
                Task novalidTask = Task.Run(() => { NoValidCheck(); });
                Task nolifeTask = Task.Run(() => { NoLifeCheck(); });
                Task PremiumTask = Task.Run(() => { PremiumGTCheck(); });

                List<Task> tasks = new List<Task>();
                tasks.Add(expireTask);
                tasks.Add(agingTask);
                tasks.Add(fifoTask);
                tasks.Add(novalidTask);
                tasks.Add(nolifeTask);
                tasks.Add(PremiumTask);
                Task t = Task.WhenAll(tasks.ToArray());

                try
                {
                    t.Wait();
                }
                catch (Exception exin)
                {
                    //Logger.Log("Check tasks failed: " + Environment.NewLine + exin.Message, Logger.LogState.Error);
                }

                AllCheck = FinalCheck();

            }
            catch (Exception ex)
            {
                AllCheck = false;
               // Logger.Log("Input material check: " + ex.Message, Logger.LogState.Error);
            }

        }

        private bool FinalCheck()
        {
            bool result = true;

            if (!NoLifeCheckResult || !NoValidCheckResult)
                result = false;

            return result;
        }

        private bool ExpireCheck()
        {
            bool result = false;
            result = _lotInfo.ExpiryDateResult;

            //Logger.Log("Expire check: " + result);
            ExpireCheckResult = result;
            return result;
        }

        private bool AgingCheck()
        {
            bool result = false;

            DataTable dt = dbglob.AgingCheck(_LOT_ID);
            if (dt.Rows.Count > 0)
            {
                string itemKind = dt.Rows[0]["ITEM_KIND"].ToString();
                string agingTime = dt.Rows[0]["AGING_TIME"].ToString();

                if ((agingTime == "OK") || (agingTime == "NG" && (itemKind == "M23" || itemKind == "Z23" || itemKind == "Z11")))
                    result = true;
            }

           // Logger.Log("Aging check: " + result);
            AgingCheckResult = result;
            return result;
        }

        private bool FIFOCheck()
        {
            bool result = false;

            DataTable dt = dbglob.FIFOCheck(_LOT_ID);
            if (dt.Rows.Count > 0)
                result = dt.Rows[0][0].ToString() == "O" ? true : false;

           // Logger.Log("FIFO check: " + result);
            FIFOCheckResult = result;
            return result;
        }

        // Specification check
        private bool NoValidCheck()
        {
            bool result = false;

            if (_workOrder.ITEM_ID != null)
            {
                DataTable dt = dbglob.NoValidCheck(
                    _EQ_ID,
                    _LOT_ID,
                    _workOrder.WO_NO,
                    _workOrder.WO_ChildItemID,
                    _workOrder.WO_ChildItemName,
                    _workOrder.WO_ChildItemCompound
                    );

                // "Logic" from old minipc
                if (dt.Rows.Count > 0)
                    result = !string.IsNullOrEmpty(dt.Rows[0]["LOT_ID"].ToString());
                else
                    result = _lotInfo.Kind == "Z23" ? true : false;
            }

           // Logger.Log("NoValid check: " + result);
            NoValidCheckResult = result;
            return result;
        }

        // Enabled / disabled item
        private bool NoLifeCheck()
        {
            bool result = false;

            DataTable dt = dbglob.NoLifeCheck(_LOT_ID);

            if (dt.Rows.Count > 0 && dt.Rows[0]["LIFE_YN"].ToString() == "Y")
                result = true;


          // Logger.Log("No life check: " + result);
            NoLifeCheckResult = result;
            return result;
        }

        private bool PremiumGTCheck()
        {
            bool result = false;

            if (_lotInfo.Division == "D")
            {
                DataTable dt = dbglob.GetPremiumGtInfo(_lotInfo.ID);

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["D_ONLY_YN"].ToString() != "Y")
                        result = true;
                }
                else
                {
                    result = true;
                }
            }
            else
            {
                result = true;
            }

           // Logger.Log("Premium GT check: " + result);
            PremiumGtResult = result;
            return result;
        }

        private bool IsGtDOnly()
        {
            bool result = false;

            DataTable dt = dbglob.GetPremiumGtInfo(_lotInfo.ID);
            if (dt.Rows.Count > 0 && dt.Rows[0]["D_ONLY_YN"].ToString() == "Y")
                result = true;

          //  Logger.Log("GT division check: " + result);
            IsGtDOnlyResult = result;
            return result;
        }

    }
}
