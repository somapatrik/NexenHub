using NexenHub.Class;
using NexenHub.Models;
using System.Collections.Generic;
using System.Linq;

namespace NexenHub.ViewModels
{
    public class InputPositionsViewModel
    {
        public string EQ_ID { get; set; }
        public List<EQPOS> InputPositions { get; set; }

        private GlobalDatabase database = new GlobalDatabase();


        public InputPositionsViewModel(string eqid)
        {
            EQ_ID = eqid;
            LoadInputPositions();
        }

        private void LoadInputPositions()
        {
            InputPositions = database.GetInputPositions(EQ_ID).OrderBy(x => x.IO_POSID).ToList();
        }
    }
}
