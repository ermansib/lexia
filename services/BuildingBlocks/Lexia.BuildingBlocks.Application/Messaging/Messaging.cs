using Lexia.BuildingBlocks.Domain;

namespace Lexia.BuildingBlocks.Application.Messaging;

/// <summary>A request that changes state and returns no value.</summary>
public interface ICommand;

/// <summary>A request that changes state and returns a value.</summary>
public interface ICommand<TResponse>;

/// <summary>A request that only reads state.</summary>
public interface IQuery<TResponse>;

public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<Result> Handle(TCommand command, CancellationToken ct);
}

public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<Result<TResponse>> Handle(TCommand command, CancellationToken ct);
}

public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken ct);
}
