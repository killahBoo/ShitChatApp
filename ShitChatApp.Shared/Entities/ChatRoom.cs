using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShitChatApp.Shared.Entities
{
	public class ChatRoom
	{
        public int ChatRoomID { get; set; }
        public required string RoomName { get; set; }
        public int RoomCode { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
