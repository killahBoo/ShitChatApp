using Microsoft.EntityFrameworkCore;
using ShitChatApp.Shared.Entities;

namespace ShitChatApp.Data
{
	public class RoomRepo(DataContext context)
	{
		private readonly DataContext _context = context;

		public async Task CreateRoom(ChatRoom newRoom)
		{
			await _context.ChatRooms.AddAsync(newRoom);
			await _context.SaveChangesAsync();
		}

		public async Task<List<ChatRoom>> GetRooms()
		{
			return await _context.ChatRooms.ToListAsync();
		}

		public async Task<ChatRoom> FindRoom(string roomId)
		{
			return await _context.ChatRooms.SingleOrDefaultAsync(r => r.ChatRoomID == roomId);
		}

		public async Task<User> GetUser(string username)
		{
			return await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);
		}

		public async Task<ChatRoom> UpdateRoom(ChatRoom updatedRoom)
		{
			var oldRoom = await FindRoom(updatedRoom.ChatRoomID);
			_context.Entry(oldRoom).CurrentValues.SetValues(updatedRoom);
			await _context.SaveChangesAsync();
			return updatedRoom;
		}

		public async Task SaveMessage(Message message)
		{
			_context.Messages.Add(message);
			await _context.SaveChangesAsync();
		}
	}
}
