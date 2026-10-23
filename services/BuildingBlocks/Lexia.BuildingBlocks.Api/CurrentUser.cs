using System.Security.Claims;
using Lexia.BuildingBlocks.Application.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Lexia.BuildingBlocks.Api;

internal sealed class HttpCurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public Guid Id
    {
        get
        {
            var principal = accessor.HttpContext?.User;
            var subject = principal?.FindFirstValue(ClaimTypes.NameIdentifier) ?? principal?.FindFirstValue("sub");
            return Guid.TryParse(subject, out var id)
                ? id
                : throw new InvalidOperationException("The request has no authenticated user with a GUID subject.");
        }
    }
}

internal sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
