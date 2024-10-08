using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShitChatApp.Client.Pages;
using ShitChatApp.Client.Services;
using ShitChatApp.Components;
using ShitChatApp.Data;
using ShitChatApp.Hubs;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.SessionStorage;
using System.Text.Json.Serialization;
using Serilog;

//Logg
Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Debug()
	.WriteTo.Console()
	.WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, buffered: true)
	.CreateLogger();

try
{
	Log.Information("Starting chat application...");

	var builder = WebApplication.CreateBuilder(args);

	// Add services to the container.
	builder.Services.AddRazorComponents()
		.AddInteractiveWebAssemblyComponents();

	//SignalR
	builder.Services.AddSignalR();

	builder.Services.AddControllers().AddJsonOptions(opt => opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

	//Session storage
	builder.Services.AddBlazoredSessionStorage();

	//DbContext
	builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ChatDB")));

	//Kestrel, enforce HTTPS
	builder.WebHost.ConfigureKestrel(options =>
	{
		options.ListenAnyIP(7093, listenopt =>
		{
			listenopt.UseHttps();
		});
	});

	builder.Services.AddScoped<Repo>();
	builder.Services.AddScoped<IAuthService, AuthService>();
	builder.Services.AddHttpClient();

	builder.Services.AddControllers();
	builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
		{
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = false,
				ValidateAudience = false,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("mysecretKey12345!#123456789101112"))
			};
			options.Events = new JwtBearerEvents
			{
				OnMessageReceived = context =>
				{
					var accessToken = context.Request.Query["access_token"];

					if (!string.IsNullOrEmpty(accessToken) && context.HttpContext.Request.Path.StartsWithSegments("/chathub"))
					{
						context.Token = accessToken;
					}
					return Task.CompletedTask;
				}
			};
		});

	builder.Services.AddAuthorization();

	var app = builder.Build();

	// Configure the HTTP request pipeline.
	if (app.Environment.IsDevelopment())
	{
		app.UseWebAssemblyDebugging();
	}
	else
	{
		app.UseExceptionHandler("/Error", createScopeForErrors: true);
		// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
		app.UseHsts();
	}

	app.UseHttpsRedirection();

	app.UseStaticFiles();
	app.UseAntiforgery();
	app.UseAuthentication();
	app.UseAuthorization();

	app.MapHub<ChatHub>("/chathub");
	app.MapControllers();

	app.MapRazorComponents<App>()
		.AddInteractiveWebAssemblyRenderMode()
		.AddAdditionalAssemblies(typeof(ShitChatApp.Client._Imports).Assembly);

	app.Run();

}
catch (Exception ex)
{
	Log.Fatal(ex, "Unexpected termination");
}
finally
{
	Log.CloseAndFlush();
}