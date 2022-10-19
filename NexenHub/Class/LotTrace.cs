using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Class
{
    public class LotTrace
    {
        public string LOT_ID { get; set; }

        public List<LotTrace> Parents { get; set; }

        public List<string> ParentsRaw { get; set; }

        private GlobalDatabase dbglob = new GlobalDatabase();

        public LotTrace(string LOT)
        {
            LOT_ID = LOT;

        }

        public async Task LoadParents()
        {
            ParentsRaw = new List<string>();
            foreach (DataRow row in dbglob.GetLotParents(LOT_ID).Rows)
                ParentsRaw.Add(row["INPUT_LOT_ID"].ToString());

            await CreateParents();

        }

        private async Task CreateParents()
        {
            Parents = new List<LotTrace>();
            foreach (string RawParent in ParentsRaw)
                Parents.Add(new LotTrace(RawParent));

            foreach (var Parent in Parents)
            {
                await Parent.LoadParents();
                //await Parent.CreateParents();
            }
        }

    }
}
