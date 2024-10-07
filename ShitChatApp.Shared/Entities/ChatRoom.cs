using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShitChatApp.Shared.Entities
{
	public class ChatRoom
	{
        public string ChatRoomID { get; set; }
        public string RoomName { get; set; }
        public int RoomCode { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
		[JsonIgnore]
        public ICollection<Message> Messages { get; set; } = new List<Message>();

		public ChatRoom() { }

		public ChatRoom(string roomId, string roomName, int roomCode)
		{ 
			ChatRoomID = roomId;
			RoomName = roomName;
			RoomCode = roomCode;
		}
	}
}
