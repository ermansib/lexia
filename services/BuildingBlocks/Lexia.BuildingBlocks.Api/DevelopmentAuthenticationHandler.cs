using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lexia.BuildingBlocks.Api;

/// <summary>
/// Signs every request in as one fixed local author. Registered only in the Development
/// environment when <c>Auth:UseDevelopmentUser</c> is true, so the app runs without Keycloak.
/// </summary>
internal sealed class DevelopmentAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "Development";
    public static readonly Guid DevelopmentUserId = Guid.Parse("00000000-0000-0000-0000-00000000a001");

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Claim[] claims =
        [
            new("sub", DevelopmentUserId.ToString()),
            new(ClaimTypes.NameIdentifier, DevelopmentUserId.ToString()),
            new(ClaimTypes.Name, "Local Author"),
        ];
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, SchemeName));
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, SchemeName)));
    }
}
