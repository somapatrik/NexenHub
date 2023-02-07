using Microsoft.Identity.Client;

namespace NexenHub.Models
{
    public class ComboItem
    {
        public string ID { get; set; }
        public string Value { get; set; }
        public string Value2 { get; set; }
        public string Value3 { get; set; }

        public ComboItem() { }
        
        public ComboItem(string ID, string Value)
        {
            this.ID = ID;
            this.Value = Value;
        }

        public ComboItem(string ID, string Value, string Value2, string Value3)
        {
            this.ID = ID;
            this.Value = Value;
            this.Value2 = Value2;
            this.Value3 = Value3;
        }
    }
}
