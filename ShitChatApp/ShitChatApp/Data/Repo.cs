using Microsoft.EntityFrameworkCore;
using ShitChatApp.Client.DTOs;
using ShitChatApp.Helpers;
using ShitChatApp.Shared.Entities;

namespace ShitChatApp.Data
{
	public class Repo(DataContext context)
	{
		private readonly DataContext _context = context;

		public async Task CreateRoom(ChatRoom newRoom)
		{
			await _context.ChatRooms.AddAsync(newRoom);
			await _context.SaveChangesAsync();
		}

		public async Task<List<ChatRoomDTO>> GetRooms()
		{
			var roomList = await _context.ChatRooms.Select(r => new ChatRoomDTO
			{
				ChatRoomID = r.ChatRoomID,
				RoomName = r.RoomName,
				RoomCode = r.RoomCode,
				Messages = r.Messages.Select(m => new MessageDTO
				{
					MessageID = m.MessageID,
					Content = m.Content,
					SentAt = m.SentAt,
					UserID = m.UserID
				}).ToList(),
				Users = r.Users.Select(u => new UserDTO
				{
					UserID = u.UserID,
					UserName = u.UserName
				}).ToList(),
			}).ToListAsync();

			foreach (var room in roomList) 
			{
				foreach (var message in room.Messages)
				{
					if (IsBase64String(message.Content))
					{
						message.Content = EncryptionHelper.Decrypt(message.Content);
					}
				}
			}
			return roomList;
		}

		private bool IsBase64String(string base64)
		{
			Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
			return base64.Length % 4 == 0 && Convert.TryFromBase64String(base64, buffer, out _);
		}

		public async Task<ChatRoom> FindRoom(string roomId)
		{
			var room = await _context.ChatRooms.SingleOrDefaultAsync(r => r.ChatRoomID == roomId);
			if (room is not null) return room;
			return null;
		}

		public async Task<UserDTO> GetUserDTO(string username)
		{
			var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);
			if (user is not null)
			{
				var userDTO = new UserDTO { UserID = user.UserID, UserName = user.UserName };
				return userDTO;
			}
			return null;
		}
		public async Task<User> GetUser(string username)
		{
			var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);
			if (user is not null) return user;
			return null;
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

		//public async Task<List<Message>> GetMessages(string roomId)
		//{
		//	return await _context.Messages.Where(m => m.ChatRoomID == roomId).ToListAsync();
		//}
	}
}
