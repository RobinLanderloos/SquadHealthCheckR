using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace SquadHealthCheckR.API.Auth.OpenIdDict;

public class AuthorizationController : Controller
{
    private readonly IOpenIddictApplicationManager _applicationManager;

    public AuthorizationController(IOpenIddictApplicationManager applicationManager)
    {
        _applicationManager = applicationManager;
    }

    [HttpPost("~/connect/token")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest();

        if (request == null)
        {
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");
        }

        if (!request.IsClientCredentialsGrantType())
            throw new NotImplementedException("The specified grant is not implemented.");

        // Note: the client credentials are automatically validated by OpenIddict:
        // if client_id or client_secret are invalid, this action won't be invoked.

        if (request.ClientId is null)
            throw new InvalidOperationException("The client_id parameter cannot be null.");

        var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
                          throw new InvalidOperationException("The application cannot be found.");

        // Create a new ClaimsIdentity containing the claims that
        // will be used to create an id_token, a token or a code.
        var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType,
            OpenIddictConstants.Claims.Name,
            OpenIddictConstants.Claims.Role);

        // Use the client_id as the subject identifier.
        identity.SetClaim(OpenIddictConstants.Claims.Subject, await _applicationManager.GetClientIdAsync(application));
        identity.SetClaim(OpenIddictConstants.Claims.Name, await _applicationManager.GetDisplayNameAsync(application));

        identity.SetDestinations(static claim => claim.Type switch
        {
            // Allow the "name" claim to be stored in both the access and identity tokens
            // when the "profile" scope was granted (by calling principal.SetScopes(...)).
            OpenIddictConstants.Claims.Name when
                claim.Subject?.HasScope(OpenIddictConstants.Permissions.Scopes.Profile) ??
                throw new InvalidOperationException("The subject cannot be null.")
                => [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken],

            // Otherwise, only store the claim in the access tokens.
            _ => [OpenIddictConstants.Destinations.AccessToken]
        });

        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}