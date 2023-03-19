namespace ChatChallenge.WebApp.Data.Model
{
    public class OnlineUserInGroup
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public string ConnectionId { get; set; } = string.Empty;
    }
}
