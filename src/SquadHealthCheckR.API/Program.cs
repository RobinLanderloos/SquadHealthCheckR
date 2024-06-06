using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SquadHealthCheckR.API.Data;
using SquadHealthCheckR.API.Domain;
using SquadHealthCheckR.API.Mailing;
using SquadHealthCheckR.API.UseCases.Admin;
using SquadHealthCheckR.API.UseCases.Session;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SquadHealthCheckR.API.Auth;
using SquadHealthCheckR.API.Bootstrapper;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.logging.json")
    .AddJsonFile("appsettings.secrets.json");
//https://learn.microsoft.com/en-us/aspnet/core/blazor/security/webassembly/?view=aspnetcore-8.0
builder.Services.AddSerilog(cfg => { cfg.ReadFrom.Configuration(builder.Configuration); });

var t = builder.Configuration.GetConnectionString("postgres");

builder.Services.AddDbContext<NpgsqlApplicationDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("postgres"));
});

builder.Services.AddIdentityCore<ApplicationUser>(opt => { opt.SignIn.RequireConfirmedEmail = true; })
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

RouteGroupBuilder accountGroup = app.MapGroup("/account");
accountGroup.MapIdentityApi<ApplicationUser>();
accountGroup.MapGet("/roles", (ClaimsPrincipal user) =>
{
    if (user.Identity is null || !user.Identity.IsAuthenticated)
    {
        return Results.Unauthorized();
    }

    var identity = (ClaimsIdentity)user.Identity;
    var roles = identity.FindAll(identity.RoleClaimType)
        .Select(c =>
            new
            {
                c.Issuer,
                c.OriginalIssuer,
                c.Type,
                c.Value,
                c.ValueType
            });

    return TypedResults.Json(roles);
}).RequireAuthorization();
accountGroup.MapPost("/logout", async (SignInManager<ApplicationUser> signInManager, [FromBody] object? empty) =>
{
    if (empty is null) return Results.Unauthorized();

    await signInManager.SignOutAsync();

    return Results.Ok();
}).RequireAuthorization();

app.MapGroup("/session")
    .MapCreateSessionEndpoint()
    .MapDeleteSessionEndpoint();

app.MapGroup("/admin")
    .MapGetSessionsEndpoint()
    .RequireAuthorization(AuthorizationPolicies.AdminOnly);

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<NpgsqlApplicationDbContext>();
await dbContext.Database.EnsureCreatedAsync();

await AdminBootstrapper.InitializeAdminUserAndRoleIfNotExists(app);

app.Run();
return;

