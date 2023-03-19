using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChatChallenge.WebApp.Pages
{
    [BindProperties]
    public class ChatGroupRoomModel : PageModel
    {
        public string RoomId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;


        public void OnGet(int? roomID)
        {
            RoomId = roomID.ToString();

            if (User.Identity is not null)
            {
                UserName = User.Identity.Name!.Split('@')[0];
            }
        }
    }
}
