using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ShitChatApp.Data;
using ShitChatApp.Shared.Entities;

namespace ShitChatApp.Hubs
{
	[Authorize]
	public class ChatHub : Hub
	{
		private static List<ChatRoom> roomList = new();
		private User User = new();
		private readonly Repo _roomRepo;

		public ChatHub(Repo repo)
		{
			_roomRepo = repo;
		}

		public override async Task OnConnectedAsync()
		{
			roomList = await _roomRepo.GetRooms();
			Console.WriteLine("Rooms: " + roomList.Count);
			User = await GetUser();
			await Clients.Caller.SendAsync("GetRooms", roomList, User);
		}

		public async Task SendMessage(ChatRoom room, User user, string message)
		{
			//skapar nytt meddelande och sparar i databasen
			var newMessage = new Message(room.ChatRoomID, user.UserID, message, DateTime.UtcNow);
			await _roomRepo.SaveMessage(newMessage);
			newMessage.User = user;
			await Clients.Group(room.ChatRoomID).SendAsync("RecieveMessage", newMessage);
			
		}

		public async Task<ChatRoom> CreateNewRoom(string roomName, int roomCode)
		{
			//skapar och lägger till nytt rum i databasen
			var roomId = Guid.NewGuid().ToString();
			var user = await GetUser();
			var newRoom = new ChatRoom(roomId, roomName, roomCode);
			newRoom.Users.Add(user);
			await _roomRepo.CreateRoom(newRoom);
			roomList = await _roomRepo.GetRooms();

			//lägger till inloggad user till rummet(?)
			await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
			//skickar rum-lista till alla clients(?)
			await Clients.All.SendAsync("GetRooms", roomList);

			return newRoom;
		}

		public async Task<ChatRoom> JoinRoom(ChatRoom reqRoom)
		{
			var user = await GetUser();
			var room = roomList.SingleOrDefault(r => r.ChatRoomID == reqRoom.ChatRoomID);
			if (room is not null)
			{
				//hämta gamla meddelanden och skicka med
				room.Users.Add(user);
				Console.WriteLine("In hub, users in room: " + room.Users.Count);
				await Groups.AddToGroupAsync(Context.ConnectionId, room.ChatRoomID);
				await Clients.Group(room.ChatRoomID).SendAsync("UserJoined", user);
				return room;
			}
			return null;
		}

		public async Task<User> GetUser()
		{
			var name = Context.User?.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name)!.Value;
			var user = await _roomRepo.GetUser(name);
			return user;
		}
	}
}
