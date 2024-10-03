using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ShitChatApp.Client.DTOs;
using ShitChatApp.Data;
using ShitChatApp.Shared.Entities;

namespace ShitChatApp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly DataContext _context;

		public AuthController(DataContext context)
		{
			_context = context;
		}

		[HttpPost("signup")]
		public async Task<IActionResult> Signup(SignupDTO user)
		{
			if (user is not null) 
			{
				var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
				var newUser = new User {UserName = user.UserName, PasswordHash = hashedPassword };

				await _context.Users.AddAsync(newUser);
				await _context.SaveChangesAsync();

				return Ok("User created successfully");
			}
			else
			{
				return BadRequest("Bad input");
			}
		}

		[HttpPost("signin")]
		public async Task<IActionResult> SignIn(SignupDTO user)
		{
			var realUser = _context.Users.SingleOrDefault(u => u.UserName == user.UserName);
			if (user == null || !BCrypt.Net.BCrypt.Verify(user.Password, realUser.PasswordHash))
			{
				return Unauthorized("Invalid login");
			}

			var token = GenerateJwtToken(realUser);
			return Ok(token);
		}

		private string GenerateJwtToken(User user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes("mysecretKey12345!#123456789101112");

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[] 
				{
					new Claim(ClaimTypes.Name, user.UserName)
				}),
				Expires = DateTime.UtcNow.AddHours(2),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}
