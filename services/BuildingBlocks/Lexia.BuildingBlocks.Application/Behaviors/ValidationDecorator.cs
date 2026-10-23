using FluentValidation;
using FluentValidation.Results;
using Lexia.BuildingBlocks.Application.Messaging;
using Lexia.BuildingBlocks.Domain;

namespace Lexia.BuildingBlocks.Application.Behaviors;

internal static class ValidationRunner
{
    public static async Task<ValidationError?> ValidateAsync<T>(T request, IEnumerable<IValidator<T>> validators, CancellationToken ct)
    {
        var context = new ValidationContext<T>(request);
        var results = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, ct)));
        List<ValidationFailure> failures = [.. results.SelectMany(r => r.Errors)];

        return failures.Count == 0
            ? null
            : new ValidationError(failures
                .GroupBy(f => f.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).ToArray()));
    }
}

internal sealed class ValidationDecorator<TCommand>(
    ICommandHandler<TCommand> inner,
    IEnumerable<IValidator<TCommand>> validators) : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    public async Task<Result> Handle(TCommand command, CancellationToken ct)
    {
        var error = await ValidationRunner.ValidateAsync(command, validators, ct);
        return error is null ? await inner.Handle(command, ct) : Result.Failure(error);
    }
}

internal sealed class ValidationDecorator<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> inner,
    IEnumerable<IValidator<TCommand>> validators) : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken ct)
    {
        var error = await ValidationRunner.ValidateAsync(command, validators, ct);
        return error is null ? await inner.Handle(command, ct) : Result.Failure<TResponse>(error);
    }
}

internal sealed class QueryValidationDecorator<TQuery, TResponse>(
    IQueryHandler<TQuery, TResponse> inner,
    IEnumerable<IValidator<TQuery>> validators) : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken ct)
    {
        var error = await ValidationRunner.ValidateAsync(query, validators, ct);
        return error is null ? await inner.Handle(query, ct) : Result.Failure<TResponse>(error);
    }
}
