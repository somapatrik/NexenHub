using System.Collections.Generic;

namespace NexenHub.Models
{
    public class ChartObject
    {
            public string Title { get; set; }
            public List<string> Labels { get; set; }
            public List<ChartDataSet> DataSets { get; set; }

            public ChartObject(string title)
            {
                Title = title;
                DataSets = new List<ChartDataSet>();
                Labels = new List<string>();
            }

            public void AddLabel(string label)
            {
                if (!Labels.Contains(label))
                    Labels.Add(label);
            }

            public void AddDataSet(string dsName)
            {
                if (DataSets.Find(x => x.Label == dsName) == null)
                {
                    ChartDataSet dSet = new ChartDataSet() { Label = dsName };
                    Labels.ForEach(x => dSet.Data.Add("NaN"));
                    DataSets.Add(dSet);
                }
            }

            public void AddValue(string dSet, string label, string value)
            {
                int i = Labels.FindIndex(l => l == label);
                try
                {
                    DataSets.Find(d => d.Label == dSet).Data[i] = value;
                }
                catch { }
            }

        

        public class ChartDataSet
        {
            public string Label { get; set; }
            public List<string> Data { get; set; }

            public ChartDataSet()
            {
                Data = new List<string>();
            }

        }
    }
}
