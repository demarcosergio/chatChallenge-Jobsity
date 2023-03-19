using ChatChallenge.WebApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChatChallenge.WebApp.Pages
{
    public class ChatGroupModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public string UserName { get; set; } = string.Empty;
        public string UsersInGroup1 { get; set; } = string.Empty;
        public string UsersInGroup2 { get; set; } = string.Empty;
        public string UsersInGroup3 { get; set; } = string.Empty;
        public string UsersInGroup4 { get; set; } = string.Empty;
        public ChatGroupModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            if (User.Identity is not null)
            {
                UserName = User.Identity.Name!.Split('@')[0];
            }
            UsersInGroup1 = SetUsersConnectedToRoom("1");
            UsersInGroup2 = SetUsersConnectedToRoom("2");
            UsersInGroup3 = SetUsersConnectedToRoom("3");
            UsersInGroup4 = SetUsersConnectedToRoom("4");
        }

        private string SetUsersConnectedToRoom(string groupId)
        {
            var users = _context.OnlineUserInGroups.Where(x=>x.GroupId == groupId).ToList();
            return String.Join(",", users.Select(x => x.UserName).ToList());
        }
    }
}
