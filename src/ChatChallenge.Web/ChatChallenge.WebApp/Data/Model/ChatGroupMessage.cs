namespace ChatChallenge.WebApp.Data.Model
{
    public class ChatGroupMessage
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public string MessageText { get; set; } = string.Empty;
        public DateTime MessageDateTime { get; set; }
    }
}
