using ChatChallenge.WebApp.Data;
using ChatChallenge.WebApp.Util;
using ChatChallenge.Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ChatChallenge.WebApp.Data.Model;
using System.Linq;

namespace ChatChallenge.WebApp.Hubs
{
	public class ChatHub : Hub
	{
		private readonly IBus _bus;
		private readonly ApplicationDbContext _context;

		public ChatHub(IBus bus, ApplicationDbContext context)
		{
			_bus = bus;
			_context = context;
		}

		public async Task Send(string userName, string message)
		{
			var chatMessage = new Contracts.ChatMessage
			{
				UserName = userName,
				MessageText = message,
				MessageDateTime = DateTime.Now
			};

			List<Task> tasks = new();

			if (!CommandIdentifier.MessageHasStockCommands(message))
			{
				await _context.ChatMessages.AddAsync(new Data.Model.ChatMessage
				{
					UserName = chatMessage.UserName,
					MessageText = chatMessage.MessageText,
					MessageDateTime = chatMessage.MessageDateTime
				});

				tasks.Add(_context.SaveChangesAsync());
			}
			else
			{
				tasks.Add(_bus.Publish(new BotMessage
				{
					UserName = userName,
					MessageText = message,
					MessageDateTime = DateTime.Now
				}));
			}

			// Send the message from client to signalR
			tasks.Add(Clients.All.SendAsync("broadcastMessage", chatMessage));

			Task.WaitAll(tasks.ToArray());
		}

		public async Task RetriveChatHistory()
		{
			var history = _context.ChatMessages.OrderByDescending(x => x.MessageDateTime).AsNoTracking().Take(50).ToList();
			await Clients.Caller.SendAsync("loadChatHistory", history.Reverse<Data.Model.ChatMessage>());
		}
		public async Task RetriveGroupChatHistory(string groupId)
		{
			var history = _context.ChatGroupMessages.Where(x => x.GroupId == groupId).OrderByDescending(x => x.MessageDateTime).AsNoTracking().Take(50).ToList();
			await Clients.Caller.SendAsync("loadGroupChatHistory", history.Reverse<Data.Model.ChatGroupMessage>());
		}
		public async Task RetriveListOfUsers(string userName, string group)
		{
			var connectedUsers = _context.OnlineUserInGroups.Where(x => x.GroupId == group).ToList();
			List<UsersInGroup> users = new List<UsersInGroup>();
			users = (connectedUsers.Select(user => new UsersInGroup() { UserName = user.UserName })).ToList();
			users.Add(new UsersInGroup() { UserName = userName });
			await Clients.Caller.SendAsync("loadUsersInGroup", users.Reverse<Data.Model.UsersInGroup>());
		}
		public Task JoinGroup(string userName, string group)
		{
			_context.OnlineUserInGroups.Add(new OnlineUserInGroup()
			{
				GroupId = group,
				UserName = userName,
				ConnectionId = Context.ConnectionId
			});
			_context.SaveChanges();
			var chatMessage = new Contracts.ChatMessage
			{
				UserName = string.Empty,
				MessageText = $"{userName} Joined to the room",
				MessageDateTime = DateTime.Now
			};
			Clients.Group(group).SendAsync("broadcastGroupJoin", userName);
			Clients.Group(group).SendAsync("broadcastGroupMessage", chatMessage);
			return Groups.AddToGroupAsync(Context.ConnectionId, group);
		}
		public Task SendMessageToGroup(string userName, string group, string message)
		{
			var chatMessage = new Contracts.ChatMessage
			{
				UserName = userName,
				MessageText = message,
				MessageDateTime = DateTime.Now
			};
			_context.ChatGroupMessages.Add(new ChatGroupMessage()
			{
				UserName = chatMessage.UserName,
				MessageText = chatMessage.MessageText,
				MessageDateTime = DateTime.Now,
				GroupId = group,
			});
			_context.SaveChanges();

			return Clients.Group(group).SendAsync("broadcastGroupMessage", chatMessage);
		}
		public override async Task OnDisconnectedAsync(Exception ex)
		{
			//remove from DB
			var onlineUser = _context.OnlineUserInGroups.Where(x => x.ConnectionId == Context.ConnectionId).FirstOrDefault();

			if (onlineUser != null)
			{
				var onlineNumberOfUsers = _context.OnlineUserInGroups.Where(x => x.GroupId == onlineUser.GroupId).Count();

				if (onlineNumberOfUsers == 1)
				{
					var groupMessages = _context.ChatGroupMessages.Where(x => x.GroupId == onlineUser.GroupId).ToList();
					_context.ChatGroupMessages.RemoveRange(groupMessages);
				}
				_context.Remove(onlineUser);
				_context.SaveChanges();
			}
			await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
			await base.OnDisconnectedAsync(ex);
		}
	}
}