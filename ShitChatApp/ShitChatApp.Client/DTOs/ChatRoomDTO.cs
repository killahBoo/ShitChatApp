using ShitChatApp.Shared.Entities;

namespace ShitChatApp.Client.DTOs
{
	public class ChatRoomDTO
	{
		public string ChatRoomID { get; set; }
		public string RoomName { get; set; }
		public int RoomCode { get; set; }
		public List<MessageDTO> Messages { get; set; } = new List<MessageDTO>();
		public List<UserDTO> Users { get; set; }
	}
}
