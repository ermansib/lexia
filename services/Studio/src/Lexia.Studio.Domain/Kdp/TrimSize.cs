namespace Lexia.Studio.Domain.Kdp;

/// <summary>A paperback trim size supported by KDP, in inches.</summary>
public sealed record TrimSize(string Id, decimal WidthIn, decimal HeightIn, string Label)
{
    private const decimal RegularMaxWidthIn = 6.12m;
    private const decimal RegularMaxHeightIn = 9m;

    /// <summary>KDP treats anything wider than 6.12" or taller than 9" as a large trim, which lowers page limits.</summary>
    public bool IsLarge => WidthIn > RegularMaxWidthIn || HeightIn > RegularMaxHeightIn;

    public static IReadOnlyList<TrimSize> All { get; } =
    [
        new("5x8", 5m, 8m, "5\" x 8\""),
        new("5.06x7.81", 5.06m, 7.81m, "5.06\" x 7.81\""),
        new("5.25x8", 5.25m, 8m, "5.25\" x 8\""),
        new("5.5x8.5", 5.5m, 8.5m, "5.5\" x 8.5\""),
        new("6x9", 6m, 9m, "6\" x 9\" (most popular)"),
        new("6.14x9.21", 6.14m, 9.21m, "6.14\" x 9.21\""),
        new("6.69x9.61", 6.69m, 9.61m, "6.69\" x 9.61\""),
        new("7x10", 7m, 10m, "7\" x 10\""),
        new("7.44x9.69", 7.44m, 9.69m, "7.44\" x 9.69\""),
        new("7.5x9.25", 7.5m, 9.25m, "7.5\" x 9.25\""),
        new("8x10", 8m, 10m, "8\" x 10\""),
        new("8.25x6", 8.25m, 6m, "8.25\" x 6\" (landscape)"),
        new("8.25x8.25", 8.25m, 8.25m, "8.25\" x 8.25\""),
        new("8.5x8.5", 8.5m, 8.5m, "8.5\" x 8.5\""),
        new("8.5x11", 8.5m, 11m, "8.5\" x 11\" (activity books)"),
        new("8.27x11.69", 8.27m, 11.69m, "8.27\" x 11.69\" (A4)"),
    ];

    public static TrimSize? Find(string id) =>
        All.FirstOrDefault(t => string.Equals(t.Id, id, StringComparison.OrdinalIgnoreCase));
}
