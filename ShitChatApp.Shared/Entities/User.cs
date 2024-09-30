using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShitChatApp.Shared.Entities
{
	public class User
	{
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }

        public ICollection<ChatRoom> ChatRooms { get; set; } = new List<ChatRoom>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
