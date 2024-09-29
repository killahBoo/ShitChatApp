using Microsoft.EntityFrameworkCore;
using ShitChatApp.Shared.Entities;

namespace ShitChatApp.Data
{
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions options) : base(options)
		{
		}

		public DbSet<User> Users { get; set; }
		public DbSet<ChatRoom> ChatRooms { get; set; }
		public DbSet<Message> Messages { get; set; }
		
	}
}
