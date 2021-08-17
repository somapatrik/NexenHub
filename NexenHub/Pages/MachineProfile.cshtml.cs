using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NexenHub.Class;

namespace NexenHub.Pages
{
    public class ChartDataSet
    {        
        public List<string> data { get { return _data; } set { _data = value; } }
        public List<string> backgroundColor { get { return _backgroundColor; } set { _backgroundColor = value; } }
        public List<string> borderColor { get { return _borderColor; } set { _borderColor = value; } }
        public int borderWidth { get; set; }

        private List<string> _data = new List<string>();
        private List<string> _backgroundColor = new List<string>();
        private List<string> _borderColor = new List<string>();

        private bool _AddFirst;
        private List<int> _r = new List<int>();
        private List<int> _g = new List<int>();
        private List<int> _b = new List<int>();

        // 28 - 168
        private KnownColor LastColor = 0;

        private KnownColor StartColor = KnownColor.DarkMagenta;

        public ChartDataSet()
        {
            borderWidth = 1;
        }

        public void AddFirst(string data, KnownColor color)
        {
            _AddFirst = true;
            Add(data, color);
            _AddFirst = false;
        }

        public void AddFirst(string data, int r, int g, int b)
        {
            _AddFirst = true;
            Add(data, r, g, b);
            _AddFirst = false;
        }

        public void AddFirst(string data)
        {
            _AddFirst = true;
            Add(data);
            _AddFirst = false;
        }

        public void Add(string data, KnownColor color)
        {
            Color col = Color.FromKnownColor(color);
            AddData(data);
            AddDefinedColor(col.R, col.G, col.B);
        }

        public void Add(string data, int r, int g, int b)
        {
            AddData(data);
            AddDefinedColor(r, g, b);
        }

        public void Add(string data)
        {
            AddRandomColor();
            AddData(data);
        }

        private void AddData(string data)
        {
            if (_AddFirst)
                this.data.Insert(0, data);
            else
                this.data.Add(data);
        }

        private void AddRandomColor()
        {
            int r;
            int g;
            int b;

            if (LastColor > 0)
            {
                LastColor += 7;
                LastColor = (int)LastColor > 168 ? LastColor - 140 : LastColor;

                Color NewColor = Color.FromKnownColor(LastColor);
                r = NewColor.R;
                g = NewColor.G;
                b = NewColor.B;
            }
            else
            {
                Color FirstColor = Color.FromKnownColor(StartColor);
                r = FirstColor.R;
                g = FirstColor.G;
                b = FirstColor.B;
                LastColor = StartColor;
            }

                AddDefinedColor(r, g, b);
        }

        private void AddDefinedColor(int r, int g, int b)
        {
            string background_color = string.Format("rgba({0},{1},{2},0.2)", r, g, b);
            string border_color = string.Format("rgba({0},{1},{2},1)", r, g, b);

            _r.Add(r);
            _g.Add(g);
            _b.Add(b);

            if (_AddFirst)
            {
                this.backgroundColor.Insert(0,background_color);
                this.borderColor.Insert(0,border_color);
            }
            else
            {
                this.backgroundColor.Add(background_color);
                this.borderColor.Add(border_color);
            }
        }
    }

    public class MachineProfileModel : PageModel
    {
        public string IArg { get; set; }
        public string chartdata { get; set; }
        public string chartlabels { get; set; }
        public string chartdataset { get; set; }


        private GlobalDatabase dbglob = new GlobalDatabase();

        private JObject jdata = new JObject();

        private ChartDataSet ChartDataScript;

        private List<string> labels;


        public void OnGet(string EQ_ID)
        {
            if (EQ_ID != null)
            {
                IArg = EQ_ID;
                DataTable dt = dbglob.GetNonWorkSum(EQ_ID);

                FillDataScript(dt);
                chartdataset = JsonConvert.SerializeObject(ChartDataScript, Formatting.Indented);
                chartlabels = JsonConvert.SerializeObject(labels, Formatting.Indented);

                //chartdata = GetChartData(dt).ToString();
                //chartlabels = GetChartLabels(dt).ToString();
            }
        }

        public void FillDataScript(DataTable dt)
        {
            ChartDataScript = new ChartDataSet();
            labels = new List<string>();

            float sumtime = 0;

            foreach (DataRow row in dt.Rows)
            {
                string stime = row[0].ToString();
                float time = float.Parse(stime);
                sumtime += time;
                ChartDataScript.Add(stime);
                labels.Add(row[1].ToString());
            }

            ChartDataScript.AddFirst(((12*60)-sumtime).ToString(), KnownColor.LimeGreen);
            labels.Insert(0,"Work");

        }

        public JProperty GetChartData(DataTable dt)
        {
            JArray array = new JArray();
            float sumtime = 0;
            foreach (DataRow row in dt.Rows)
            {
                float time = float.Parse(row[0].ToString());
                sumtime += time;
                array.Add(new JValue(time));
            }
            array.AddFirst((12*60) - sumtime);
            JProperty jlabels = new JProperty("data", array);
            return jlabels;
        }

        public JProperty GetChartLabels(DataTable dt)
        {
            JArray array = new JArray();
            foreach (DataRow row in dt.Rows)
            {
                array.Add(new JValue(row[1].ToString()));
            }
            array.AddFirst("Work");
            JProperty jlabels = new JProperty("labels", array);
            return jlabels;
        }

        public JProperty GetChartName()
        {
            JProperty plabel = new JProperty("label", "Nonwork");
            return plabel;
            //jdata.Add(plabel);
        }

    }
}
