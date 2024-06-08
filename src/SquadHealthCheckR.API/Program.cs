using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SquadHealthCheckR.API.Data;
using SquadHealthCheckR.API.Domain;
using SquadHealthCheckR.API.Mailing;
using SquadHealthCheckR.API.UseCases.Admin;
using SquadHealthCheckR.API.UseCases.Session;
using SquadHealthCheckR.API.Auth;
using SquadHealthCheckR.API.Bootstrapper;
using SquadHealthCheckR.API.UseCases.Account;
using SquadHealthCheckR.API.UseCases.SquadMember;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.logging.json")
    .AddJsonFile("appsettings.secrets.json");

builder.Services.AddSerilog(cfg => { cfg.ReadFrom.Configuration(builder.Configuration); });

builder.Services.AddDbContext<NpgsqlApplicationDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("postgres"));
});

builder.Services.AddIdentityCore<ApplicationUser>(opt =>
    {
        opt.SignIn.RequireConfirmedEmail = true;
        opt.Password.RequireDigit = false;
        opt.Password.RequiredLength = 8;
        opt.Password.RequireLowercase = false;
        opt.Password.RequireNonAlphanumeric = false;
        opt.Password.RequireUppercase = false;
    })
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<NpgsqlApplicationDbContext>()
    .AddApiEndpoints();

builder.Services.AddMailingServices(builder.Configuration);

builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(cfg =>
    {
        cfg.WithOrigins("http://localhost:5105/", "https://localhost:7084");
        cfg.AllowAnyMethod();
        cfg.AllowAnyHeader();
        cfg.AllowCredentials();
    });
});

builder.Services.AddAuthentication().AddCookie(IdentityConstants.ApplicationScheme, opt =>
{
    opt.Events.OnRedirectToAccessDenied = c =>
    {
        c.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.FromResult<object?>(null);
    };
});

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICurrentUserAccessor, HttpContextCurrentUserAccessor>();
builder.Services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(typeof(Program).Assembly); });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("/account")
    .MapLogoutEndpoint()
    .MapGetRolesEndpoint()
    .MapIdentityApi<ApplicationUser>();

app.MapGroup("/session")
    .MapCreateSessionEndpoint()
    .MapDeleteSessionEndpoint()
    .MapJoinSessionEndpoint()
    .MapLeaveSessionEndpoint()
    .RequireAuthorization();

app.MapGroup("/squad-member")
    .MapGetSquadMemberSessionsEndpoint()
    .RequireAuthorization();

app.MapGroup("/admin")
    .MapGetSessionsEndpoint()
    .MapDeleteUserEndpoint()
    .RequireAuthorization(AuthorizationPolicies.AdminOnly);

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<NpgsqlApplicationDbContext>();
await dbContext.Database.EnsureCreatedAsync();

await AdminBootstrapper.InitializeAdminUserAndRoleIfNotExists(app);

app.Run();