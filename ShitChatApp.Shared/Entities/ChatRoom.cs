using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShitChatApp.Shared.Entities
{
	public class ChatRoom
	{
        public string ChatRoomID { get; set; }
        public string RoomName { get; set; }
        public int RoomCode { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();

		public ChatRoom( string roomName, int roomCode)
		{ 
			RoomName = roomName;
			RoomCode = roomCode;
		}
	}
}
