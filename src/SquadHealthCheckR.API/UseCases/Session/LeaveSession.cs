using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SquadHealthCheckR.API.Data;
using SquadHealthCheckR.API.Domain;

namespace SquadHealthCheckR.API.UseCases.Session;

internal static class LeaveSession
{
    internal static RouteGroupBuilder MapLeaveSessionEndpoint(this RouteGroupBuilder builder)
    {
        builder.MapDelete("/leave",
            async Task<Results<UnauthorizedHttpResult, Ok>>(IMediator mediator, ICurrentUserAccessor currentUserAccessor, CancellationToken cancellationToken, [FromQuery] Guid sessionId) =>
            {
                var user = await currentUserAccessor.GetCurrentUser();

                if (user == null)
                {
                    return TypedResults.Unauthorized();
                }

                await mediator.Send(new LeaveSessionCommand(sessionId, user), cancellationToken);

                return TypedResults.Ok();
            }).RequireAuthorization();

        return builder;
    }

    internal record LeaveSessionCommand(Guid SessionId, ApplicationUser SquadMember) : IRequest;

    internal class LeaveSessionCommandHandler : IRequestHandler<LeaveSessionCommand>
    {
        private readonly NpgsqlApplicationDbContext _applicationDbContext;

        public LeaveSessionCommandHandler(NpgsqlApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task Handle(LeaveSessionCommand request, CancellationToken cancellationToken)
        {
            var session =
                await _applicationDbContext
                    .Sessions
                    .Include(s => s.SquadMembers)
                    .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

            if (session == null)
            {
                return;
            }

            session.RemoveSquadMember(request.SquadMember);

            await _applicationDbContext.SaveChangesAsync(cancellationToken);
        }
    }

}