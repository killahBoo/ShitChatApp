
namespace ShitChatApp.Client.Services
{
	public interface IAuthService
	{
		Task<bool> SignIn(string username, string password);
		Task<bool> SignUp(string userName, string password);
		Task<string> GetToken();
	}
}