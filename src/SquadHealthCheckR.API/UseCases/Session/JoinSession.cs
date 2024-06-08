using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SquadHealthCheckR.API.Data;
using SquadHealthCheckR.API.Domain;

namespace SquadHealthCheckR.API.UseCases.Session;

internal static class JoinSession
{
    internal static RouteGroupBuilder MapJoinSessionEndpoint(this RouteGroupBuilder builder)
    {
        builder.MapPost("/join",
            async Task<Results<UnauthorizedHttpResult, NotFound<string>, BadRequest<string>, Ok>> (IMediator mediator,
                ICurrentUserAccessor currentUserAccessor, CancellationToken cancellationToken,
                [FromQuery] string inviteCode) =>
            {
                var user = await currentUserAccessor.GetCurrentUser();

                if (user == null)
                {
                    return TypedResults.Unauthorized();
                }

                var joinResult = await mediator.Send(new JoinSessionCommand(inviteCode, user), cancellationToken);

                return joinResult switch
                {
                    JoinSessionResultStatus.Success => TypedResults.Ok(),
                    JoinSessionResultStatus.SquadMemberAlreadyJoined => TypedResults.BadRequest(
                        "SquadMember.AlreadyJoined"),
                    JoinSessionResultStatus.SessionNotFound => TypedResults.NotFound("Session.NotFound"),
                    _ => TypedResults.Ok()
                };
            }).RequireAuthorization();

        return builder;
    }

    internal record JoinSessionCommand(string InviteCode, ApplicationUser SquadMember)
        : IRequest<JoinSessionResultStatus>;

    internal class JoinSessionCommandHandler : IRequestHandler<JoinSessionCommand, JoinSessionResultStatus>
    {
        private readonly NpgsqlApplicationDbContext _applicationDbContext;

        public JoinSessionCommandHandler(NpgsqlApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<JoinSessionResultStatus> Handle(JoinSessionCommand request,
            CancellationToken cancellationToken)
        {
            var session =
                await _applicationDbContext
                    .Sessions
                    .Include(s => s.SquadMembersSessions)
                    .FirstOrDefaultAsync(s => s.InviteCode == request.InviteCode, cancellationToken);

            if (session == null)
            {
                return JoinSessionResultStatus.SessionNotFound;
            }

            if (session.SquadMembersSessions.Any(sms => sms.SquadMemberId == request.SquadMember.Id))
            {
                return JoinSessionResultStatus.SquadMemberAlreadyJoined;
            }

            session.JoinSquadMember(request.SquadMember);

            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            return JoinSessionResultStatus.Success;
        }
    }

    // TODO: Move this into the domain entity?
    internal enum JoinSessionResultStatus
    {
        Success,
        SessionNotFound,
        SquadMemberAlreadyJoined
    }
}