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
	}
}
