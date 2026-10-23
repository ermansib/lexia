using Lexia.BuildingBlocks.Application.Messaging;
using Lexia.BuildingBlocks.Domain;
using Lexia.Studio.Domain.Kdp;

namespace Lexia.Studio.Application.TrimSizes;

public sealed record TrimSizeDto(string Id, decimal WidthIn, decimal HeightIn, string Label, bool IsLarge);

public sealed record ListTrimSizesQuery : IQuery<IReadOnlyList<TrimSizeDto>>;

internal sealed class ListTrimSizesQueryHandler : IQueryHandler<ListTrimSizesQuery, IReadOnlyList<TrimSizeDto>>
{
    public Task<Result<IReadOnlyList<TrimSizeDto>>> Handle(ListTrimSizesQuery query, CancellationToken ct)
    {
        IReadOnlyList<TrimSizeDto> sizes = [.. TrimSize.All.Select(t => new TrimSizeDto(t.Id, t.WidthIn, t.HeightIn, t.Label, t.IsLarge))];
        return Task.FromResult(Result.Success(sizes));
    }
}
