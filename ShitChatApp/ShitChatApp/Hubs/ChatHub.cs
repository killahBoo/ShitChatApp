using Microsoft.AspNetCore.SignalR;
using ShitChatApp.Data;
using ShitChatApp.Shared.Entities;

namespace ShitChatApp.Hubs
{
	public class ChatHub : Hub
	{
		private List<ChatRoom> roomList = new();
		private readonly RoomRepo _roomRepo;

		public ChatHub(RoomRepo repo)
		{
			_roomRepo = repo;
		}

		public async Task SendMessage(string user, string message)
		{
			await Clients.All.SendAsync("RecieveMessage", user, message);
		}

		public async Task GetRooms()
		{
			//hämtar alla rum när man kommer till CreateRoom.razor
			roomList = await _roomRepo.GetRooms();
			await Clients.Caller.SendAsync("GetRooms", roomList);
		}

		public async Task<ChatRoom> CreateNewRoom(string roomName, int roomCode)
		{
			//skapar och lägger till nytt rum i databasen
			var roomId = Guid.NewGuid().ToString();
			var newRoom = new ChatRoom( roomName, roomCode);
			await _roomRepo.CreateRoom(newRoom);

			//lägger till inloggad user till rummet(?)
			await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
			//skickar rum-lista till alla clients(?)
			await Clients.All.SendAsync("GetRooms", roomList);

			return newRoom;
		}
	}
}
