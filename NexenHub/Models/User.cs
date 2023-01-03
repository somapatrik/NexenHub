namespace NexenHub.Models
{
    public class User
    {
        public string Name { get; set; }
        public string UserId { get; set; }
        public string CardHex { get; set; }

        public bool IsValid => !string.IsNullOrEmpty(UserId);

    }
}
