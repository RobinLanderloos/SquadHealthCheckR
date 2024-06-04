using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SquadHealthCheckR.API.Data;
using SquadHealthCheckR.API.Domain;
using SquadHealthCheckR.API.Mailing;
using SquadHealthCheckR.API.UseCases.Admin;
using SquadHealthCheckR.API.UseCases.Session;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.logging.json")
    .AddJsonFile("appsettings.secrets.json");
//https://learn.microsoft.com/en-us/aspnet/core/blazor/security/webassembly/?view=aspnetcore-8.0
builder.Services.AddSerilog(cfg => { cfg.ReadFrom.Configuration(builder.Configuration); });

builder.Services.AddDbContext<NpgsqlApplicationDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("postgres"));
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(opt => { opt.SignIn.RequireConfirmedEmail = true; })
    .AddEntityFrameworkStores<NpgsqlApplicationDbContext>()
    .AddApiEndpoints();

builder.Services.AddMailingServices(builder.Configuration);

builder.Services.AddCors();

builder.Services.AddAuthentication();

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICurrentUserAccessor, HttpContextCurrentUserAccessor>();
builder.Services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(typeof(Program).Assembly); });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("/account").MapIdentityApi<ApplicationUser>();

app.MapGroup("/session")
    .MapCreateSessionEndpoint()
    .MapDeleteSessionEndpoint();

app.MapGroup("/admin")
    .MapGetSessionsEndpoint();

await Task.Delay(5000);

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<NpgsqlApplicationDbContext>();
await dbContext.Database.EnsureCreatedAsync();

app.Run();