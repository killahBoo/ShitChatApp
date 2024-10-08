using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ShitChatApp.Client.DTOs;
using ShitChatApp.Data;
using ShitChatApp.Helpers;
using ShitChatApp.Shared.Entities;

namespace ShitChatApp.Hubs
{
	[Authorize]
	public class ChatHub : Hub
	{
		private static List<ChatRoomDTO> roomList = new();
		private UserDTO User = new();
		private readonly Repo _roomRepo;

		public ChatHub(Repo repo)
		{
			_roomRepo = repo;
		}

		public override async Task OnConnectedAsync()
		{
			roomList = await _roomRepo.GetRooms();
			Console.WriteLine("Rooms: " + roomList.Count);
			User = await GetUserDTO();
			await Clients.Caller.SendAsync("GetRooms", roomList, User);
		}

		public async Task SendMessage(ChatRoomDTO room, UserDTO user, string message)
		{
			//skapar nytt meddelande och sparar i databasen
			var encryptedMessage = EncryptionHelper.Encrypt(message);
			var newMessage = new Message(room.ChatRoomID, user.UserID, encryptedMessage, DateTime.UtcNow);
			await _roomRepo.SaveMessage(newMessage);
			//newMessage.User = user; behövs ej?
			var dtoMessage = new MessageDTO { Content = message, SentAt = DateTime.UtcNow, UserName = user.UserName };
			await Clients.Group(room.ChatRoomID).SendAsync("RecieveMessage", dtoMessage);
			
		}

		public async Task<ChatRoomDTO> CreateNewRoom(string roomName, int roomCode)
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

			return roomList.SingleOrDefault(r => r.ChatRoomID == roomId);
		}

		public async Task<ChatRoomDTO> JoinRoom(ChatRoomDTO reqRoom)
		{
			var user = await GetUserDTO();
			var room = roomList.SingleOrDefault(r => r.ChatRoomID == reqRoom.ChatRoomID);
				//await _roomRepo.FindRoom(reqRoom.ChatRoomID);
			if (room is not null)
			{
				//hämta gamla meddelanden och skicka med : de är med från början nu? Finns i roomList?
				//room.Messages = (ICollection<Message>)roomList.SingleOrDefault(r => r.ChatRoomID == room.ChatRoomID).Messages;
				room.Users.Add(user); //user behöver inte adderas här?
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

		public async Task<UserDTO> GetUserDTO()
		{
			var name = Context.User?.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name)!.Value;
			var user = await _roomRepo.GetUserDTO(name);
			return user;
		}
	}
}
