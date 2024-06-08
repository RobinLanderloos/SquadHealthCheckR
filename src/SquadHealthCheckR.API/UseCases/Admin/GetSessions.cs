using MediatR;
using Microsoft.EntityFrameworkCore;
using SquadHealthCheckR.API.Data;
using SquadHealthCheckR.API.Domain;

namespace SquadHealthCheckR.API.UseCases.Admin;

internal static class GetSessions
{
    internal static RouteGroupBuilder MapGetSessionsEndpoint(this RouteGroupBuilder builder)
    {
        builder.MapGet("/sessions", async (IMediator mediator) =>
        {
            var sessions = await mediator.Send(new GetSessionsQuery());

            return sessions;
        });
        return builder;
    }

    internal record GetSessionsQuery : IRequest<IEnumerable<SessionDto>>;

    internal class GetSessionsQueryHandler : IRequestHandler<GetSessionsQuery, IEnumerable<SessionDto>>
    {
        private readonly NpgsqlApplicationDbContext _dbContext;

        public GetSessionsQueryHandler(NpgsqlApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<SessionDto>> Handle(GetSessionsQuery request, CancellationToken cancellationToken)
        {
            var sessions = await _dbContext.Sessions
                .Select(s => new SessionDto(
                    s.Id,
                    s.Name,
                    s.SquadMembersSessions.First(sms => sms.Type == SquadMembersSessions.SessionUserType.SquadLeader)
                        .SquadMemberId,
                    s.SquadMembersSessions.Where(sms => sms.Type == SquadMembersSessions.SessionUserType.SquadMember).Select(sms => sms.SquadMemberId)))
                .ToListAsync(cancellationToken);

            return sessions;
        }
    }

    internal record SessionDto(Guid Id, string Name, Guid SquadLeaderId, IEnumerable<Guid> SquadMemberIds);
}