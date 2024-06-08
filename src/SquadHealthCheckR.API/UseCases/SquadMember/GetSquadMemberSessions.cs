using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SquadHealthCheckR.API.Data;
using SquadHealthCheckR.API.Domain;
using SquadHealthCheckR.API.UseCases.Session;

namespace SquadHealthCheckR.API.UseCases.SquadMember;

internal static class GetSquadMemberSessions
{
    internal static RouteGroupBuilder MapGetSquadMemberSessionsEndpoint(this RouteGroupBuilder builder)
    {
        builder.MapGet("/sessions",
            async Task<Results<UnauthorizedHttpResult, Ok<SquadMemberSessionDto[]>>>(IMediator mediator, ICurrentUserAccessor currentUserAccessor, CancellationToken cancellationToken) =>
            {
                var user = await currentUserAccessor.GetCurrentUser();

                if (user == null)
                {
                    return TypedResults.Unauthorized();
                }

                var sessions = await mediator.Send(new GetSessionsQuery(user), cancellationToken);

                return TypedResults.Ok(sessions.Select(s => new SquadMemberSessionDto(s.Id, s.Name, s.InviteCode)).ToArray());
            }).RequireAuthorization();

        return builder;
    }

    internal record SquadMemberSessionDto(Guid Id, string Name, string InviteCode);
    internal record GetSessionsQuery(ApplicationUser SquadMember) : IRequest<IEnumerable<SquadMemberSessionDto>>;

    internal class GetSessionsQueryHandler : IRequestHandler<GetSessionsQuery, IEnumerable<SquadMemberSessionDto>>
    {
        private readonly NpgsqlApplicationDbContext _applicationDbContext;

        public GetSessionsQueryHandler(NpgsqlApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<IEnumerable<SquadMemberSessionDto>> Handle(GetSessionsQuery request, CancellationToken cancellationToken)
        {
            var sessions = await _applicationDbContext
                .Sessions
                .Where(s => s.SquadMembers.Any(sm => sm.Id == request.SquadMember.Id))
                .Select(s => new SquadMemberSessionDto(s.Id, s.Name, s.InviteCode))
                .ToListAsync(cancellationToken);

            return sessions;
        }
    }
}