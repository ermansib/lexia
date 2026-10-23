using Lexia.BuildingBlocks.Domain;

namespace Lexia.Studio.Domain.Kdp;

public static class KdpErrors
{
    public static Error UnknownTrimSize(string id) =>
        Error.Validation("Kdp.UnknownTrimSize", $"'{id}' is not a KDP paperback trim size.");

    public static Error PageCountTooLow(int pageCount) =>
        Error.Validation("Kdp.PageCountTooLow", $"KDP paperbacks need at least {KdpPrintRules.MinPageCount} pages, but this book has {pageCount}.");

    public static Error PageCountTooHigh(int pageCount, int max) =>
        Error.Validation("Kdp.PageCountTooHigh", $"This trim size and paper allow at most {max} pages, but this book has {pageCount}.");
}
