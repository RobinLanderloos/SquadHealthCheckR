using MediatR;
using Microsoft.EntityFrameworkCore;
using SquadHealthCheckR.API.Data;

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

    internal record GetSessionsQuery : IRequest<IEnumerable<Domain.Session>>;

    internal class GetSessionsQueryHandler : IRequestHandler<GetSessionsQuery, IEnumerable<Domain.Session>>
    {
        private readonly NpgsqlApplicationDbContext _dbContext;

        public GetSessionsQueryHandler(NpgsqlApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Domain.Session>> Handle(GetSessionsQuery request, CancellationToken cancellationToken)
        {
            var sessions = await _dbContext.
                Sessions
                .Include(x => x.SquadLeader)
                .Include(x => x.SquadMembers)
                .Include(x => x.Votes)
                .Include(x => x.HealthIndicators)
                .ToListAsync(cancellationToken);

            return sessions;
        }
    }
}