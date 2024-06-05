using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SquadHealthCheckR;
using SquadHealthCheckR.Auth;
using SquadHealthCheckR.Configuration;
using SquadHealthCheckR.JavascriptInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient<CookieHandler>();
builder.Services.AddScoped<ThemingJavascriptInterop>();
builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();

// register the account management interface
builder.Services.AddScoped(
    sp => (IAccountManagement)sp.GetRequiredService<AuthenticationStateProvider>());

builder.Services.AddConfiguration(builder.Configuration);

builder.Services.AddBlazoredLocalStorage();

var baseUrl = builder.Configuration.GetValue<string>($"{Api.SectionName}:{nameof(Api.BaseUrl)}");

if (baseUrl == null)
{
    throw new ArgumentNullException(nameof(baseUrl), "Api.BaseUrl not found in configuration");
}


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseUrl) });
builder.Services.AddHttpClient(
        "Auth",
        opt => opt.BaseAddress = new Uri($"{baseUrl}/account/"))
    .AddHttpMessageHandler<CookieHandler>();

builder.Services.AddHttpClient();

builder.Services.AddAuthorizationCore();

WebAssemblyHost app = builder.Build();

await app.RunAsync();