using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ShitChatApp.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Register AuthService
builder.Services.AddScoped<IAuthService, AuthService>();

//Session storage
builder.Services.AddBlazoredSessionStorage();

await builder.Build().RunAsync();
