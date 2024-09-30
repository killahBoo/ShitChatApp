using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShitChatApp.Shared.Entities
{
	public class Message
	{
        public int MessageID { get; set; }
        public string ChatRoomID { get; set; }
        public int UserID { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public ChatRoom ChatRoom { get; set; }
        public User User { get; set; }
    }
}
