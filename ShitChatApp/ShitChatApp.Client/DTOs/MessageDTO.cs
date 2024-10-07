namespace ShitChatApp.Client.DTOs
{
	public class MessageDTO
	{
		public int MessageID { get; set; }
		public string Content { get; set; }
		public DateTime SentAt { get; set; }
		public int UserID { get; set; }
        public string UserName { get; set; }
    }
}
