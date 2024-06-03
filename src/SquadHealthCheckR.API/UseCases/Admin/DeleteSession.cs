using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SquadHealthCheckR.API.Data;

namespace SquadHealthCheckR.API.UseCases.Admin;

internal static class DeleteSession
{
    internal static RouteGroupBuilder MapDeleteSessionEndpoint(this RouteGroupBuilder builder)
    {
        builder.MapDelete("",
            async (IMediator mediator, CancellationToken cancellationToken, [FromQuery] Guid sessionId) =>
            {
                await mediator.Send(new DeleteSessionCommand(sessionId), cancellationToken);

                return TypedResults.Ok();
            });

        return builder;
    }

    internal record DeleteSessionCommand(Guid SessionId) : IRequest;

    internal class DeleteSessionCommandHandler : IRequestHandler<DeleteSessionCommand>
    {
        private readonly NpgsqlApplicationDbContext _applicationDbContext;

        public DeleteSessionCommandHandler(NpgsqlApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task Handle(DeleteSessionCommand request, CancellationToken cancellationToken)
        {
            var session =
                await _applicationDbContext.Sessions.FirstOrDefaultAsync(s => s.Id == request.SessionId,
                    cancellationToken);

            if (session == null)
            {
                // We don't care if it's not found
                return;
            }

            _applicationDbContext.Sessions.Remove(session);
            await _applicationDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}