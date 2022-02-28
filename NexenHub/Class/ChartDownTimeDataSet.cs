using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Class
{
    public class ChartDownTimeDataSet
    {
        public List<string> data { get { return _data; } set { _data = value; } }
        public List<string> backgroundColor { get { return _backgroundColor; } set { _backgroundColor = value; } }
        public List<string> borderColor { get { return _borderColor; } set { _borderColor = value; } }
        public int borderWidth { get; set; }

        private List<string> _data = new List<string>();
        private List<string> _backgroundColor = new List<string>();
        private List<string> _borderColor = new List<string>();

        private bool _AddFirst;

        // 28 - 168
        private KnownColor LastColor = 0;

        private KnownColor StartColor = KnownColor.DarkMagenta;

        public ChartDownTimeDataSet()
        {
            borderWidth = 1;
        }

        #region Add data
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

        #endregion

        #region Add color
        private void AddRandomColor()
        {
            Color SetColor;

            if (LastColor > 0)
            {
                LastColor += 1;
                LastColor = (int)LastColor > 168 ? LastColor - 140 : LastColor;
                SetColor = Color.FromKnownColor(LastColor);
            }
            else
            {
                SetColor = Color.FromKnownColor(StartColor);
                LastColor = StartColor;
            }

            int r = SetColor.R;
            int g = SetColor.G;
            int b = SetColor.B;
            AddDefinedColor(r, g, b);
        }

        private void AddDefinedColor(int r, int g, int b)
        {
            string background_color = string.Format("rgba({0},{1},{2},0.7)", r, g, b);
            string border_color = string.Format("rgba({0},{1},{2},1)", r, g, b);

            if (_AddFirst)
            {
                this.backgroundColor.Insert(0, background_color);
                this.borderColor.Insert(0, border_color);
            }
            else
            {
                this.backgroundColor.Add(background_color);
                this.borderColor.Add(border_color);
            }
        }

        #endregion
    }
}
