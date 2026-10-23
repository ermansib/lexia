using Lexia.BuildingBlocks.Application.Messaging;
using Lexia.BuildingBlocks.Domain;
using Microsoft.Extensions.Logging;

namespace Lexia.BuildingBlocks.Application.Behaviors;

internal sealed class LoggingDecorator<TCommand>(
    ICommandHandler<TCommand> inner,
    ILogger<LoggingDecorator<TCommand>> logger) : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    public async Task<Result> Handle(TCommand command, CancellationToken ct)
    {
        var result = await inner.Handle(command, ct);
        RequestLog.Write(logger, typeof(TCommand).Name, result);
        return result;
    }
}

internal sealed class LoggingDecorator<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> inner,
    ILogger<LoggingDecorator<TCommand, TResponse>> logger) : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken ct)
    {
        var result = await inner.Handle(command, ct);
        RequestLog.Write(logger, typeof(TCommand).Name, result);
        return result;
    }
}

internal static partial class RequestLog
{
    public static void Write(ILogger logger, string requestName, Result result)
    {
        if (result.IsSuccess)
        {
            Handled(logger, requestName);
        }
        else
        {
            Failed(logger, requestName, result.Error.Code);
        }
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Handled {Request}")]
    private static partial void Handled(ILogger logger, string request);

    [LoggerMessage(Level = LogLevel.Warning, Message = "{Request} failed with {ErrorCode}")]
    private static partial void Failed(ILogger logger, string request, string errorCode);
}
