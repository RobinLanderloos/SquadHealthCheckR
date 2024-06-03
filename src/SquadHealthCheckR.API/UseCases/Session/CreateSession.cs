using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SquadHealthCheckR.API.Data;
using SquadHealthCheckR.API.Domain;

namespace SquadHealthCheckR.API.UseCases.Session;

internal static class CreateSession
{
    internal static RouteGroupBuilder MapCreateSessionEndpoint(this RouteGroupBuilder builder)
    {
        builder.MapPost("", async (IMediator mediator, CreateSessionCommand command) =>
        {
            var id = await mediator.Send(command);

            return TypedResults.Ok(id);
        });

        return builder;
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    internal record CreateSessionCommand(string SessionName) : IRequest<Guid>;

    internal class CreateSessionCommandHandler : IRequestHandler<CreateSessionCommand, Guid>
    {
        private readonly NpgsqlApplicationDbContext _dbContext;
        private readonly ICurrentUserAccessor _currentUserAccessor;

        public CreateSessionCommandHandler(NpgsqlApplicationDbContext dbContext, ICurrentUserAccessor currentUserAccessor)
        {
            _dbContext = dbContext;
            _currentUserAccessor = currentUserAccessor;
        }

        public async Task<Guid> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
        {
            var user = await _currentUserAccessor.GetCurrentUser();
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            var session = new Domain.Session(null, user, request.SessionName);

            _dbContext.Sessions.Add(session);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return session.Id;
        }
    }
}

internal interface ICurrentUserAccessor
{
   Task<ApplicationUser?> GetCurrentUser();
}

internal class HttpContextCurrentUserAccessor : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;

    public HttpContextCurrentUserAccessor(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public async Task<ApplicationUser?> GetCurrentUser()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null)
        {
            return null;
        }

        // Get ID claim
        var idClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (idClaim == null)
        {
            return null;
        }

        var id = idClaim.Value;
        return await _userManager.FindByIdAsync(id);
    }
}