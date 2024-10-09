using Microsoft.EntityFrameworkCore;
using Serilog;
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
			try
			{
				await _context.ChatRooms.AddAsync(newRoom);
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				Log.Warning(ex, "ChatRoom could not be created");
			}
		}

		public async Task<List<ChatRoomDTO>> GetRooms()
		{
			try
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
						UserID = m.UserID,
						UserName = m.User.UserName
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
			catch (Exception ex)
			{
				Log.Warning(ex, "Trouble retrieving Rooms");
				return null;
			}
		}

		public bool IsBase64String(string base64)
		{
			try
			{
				Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
				return base64.Length % 4 == 0 && Convert.TryFromBase64String(base64, buffer, out _);
			}
			catch (Exception ex)
			{
				Log.Warning(ex, "Trouble in string64-method");
				throw;
			}
		}

		public async Task<ChatRoom> FindRoom(string roomId)
		{
			try
			{
				var room = await _context.ChatRooms.SingleOrDefaultAsync(r => r.ChatRoomID == roomId);
				if (room is not null) return room;
				return null;
			}
			catch (Exception ex)
			{
				Log.Warning(ex, "Could not find a room with matching id");
				return null;
			}
		}

		public async Task<UserDTO> GetUserDTO(string username)
		{
			try
			{
				var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);
				if (user is not null)
				{
					var userDTO = new UserDTO { UserID = user.UserID, UserName = user.UserName };
					return userDTO;
				}
				return null;
			}
			catch (Exception ex)
			{
				Log.Warning(ex, "Could not get user(dto)");
				return null;
			}
		}
		public async Task<User> GetUser(string username)
		{
			try
			{
				var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);
				if (user is not null) return user;
				return null;
			}
			catch (Exception ex)
			{
				Log.Warning(ex, "Could not get user");
				return null;
			}
		}

		public async Task<ChatRoom> UpdateRoom(ChatRoom updatedRoom)
		{
			try
			{
				var oldRoom = await FindRoom(updatedRoom.ChatRoomID);
				_context.Entry(oldRoom).CurrentValues.SetValues(updatedRoom);
				await _context.SaveChangesAsync();
				return updatedRoom;
			}
			catch (Exception ex)
			{
				Log.Warning(ex, "Could not update room");
				return null;
			}
		}

		public async Task SaveMessage(Message message)
		{
			try
			{
				_context.Messages.Add(message);
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				Log.Warning(ex, "Trouble saving message");
			}
		}

	}
}
