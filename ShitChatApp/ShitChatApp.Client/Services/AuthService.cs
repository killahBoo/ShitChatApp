using System.Net.Http.Json;
using Blazored.SessionStorage;
using ShitChatApp.Client.DTOs;
using ShitChatApp.Shared.Entities;

namespace ShitChatApp.Client.Services
{
	public class AuthService : IAuthService
	{
		private readonly HttpClient _httpClient;
		private readonly ISessionStorageService _sessionStorage;

		public AuthService(HttpClient httpClient, ISessionStorageService sessionStorage)
		{
			_httpClient = httpClient;
			_sessionStorage = sessionStorage;
		}

		public async Task<bool> SignIn(string username, string password)
		{
			var response = await _httpClient.PostAsJsonAsync("/api/auth/signin", new { username, password });
			if (response.IsSuccessStatusCode)
			{
				var token = await response.Content.ReadAsStringAsync();
				await _sessionStorage.SetItemAsync("jwtToken", token);
				return true;
			}
			return false;
		}

		public async Task<bool> SignUp(string userName, string password)
		{
			var userDTO = new SignupDTO{UserName = userName, Password = password};
			var response = await _httpClient.PostAsJsonAsync("api/auth/signup", userDTO);
			return response.IsSuccessStatusCode;
		}

		public async Task<string> GetToken()
		{
			return await _sessionStorage.GetItemAsync<string>("jwtToken");
		}
	}
}
