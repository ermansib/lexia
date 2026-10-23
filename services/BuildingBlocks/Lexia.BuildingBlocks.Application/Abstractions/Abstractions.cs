namespace Lexia.BuildingBlocks.Application.Abstractions;

/// <summary>The signed-in author making the current request.</summary>
public interface ICurrentUser
{
    Guid Id { get; }
}

/// <summary>Gives the current time so handlers stay testable.</summary>
public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}
