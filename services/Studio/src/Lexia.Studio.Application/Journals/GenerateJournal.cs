using FluentValidation;
using Lexia.BuildingBlocks.Application.Messaging;
using Lexia.BuildingBlocks.Domain;
using Lexia.Studio.Application.Abstractions;
using Lexia.Studio.Domain.Kdp;

namespace Lexia.Studio.Application.Journals;

public sealed record GenerateJournalCommand(
    string Title,
    string? Author,
    string TrimSizeId,
    JournalStyle Style,
    int PageCount) : ICommand<GeneratedFile>;

internal sealed class GenerateJournalCommandValidator : AbstractValidator<GenerateJournalCommand>
{
    public const int MaxTitleLength = 200;

    public GenerateJournalCommandValidator()
    {
        RuleFor(c => c.Title).NotEmpty().MaximumLength(MaxTitleLength);
        RuleFor(c => c.Author).MaximumLength(MaxTitleLength);
        RuleFor(c => c.TrimSizeId).Must(id => TrimSize.Find(id) is not null).WithMessage("Choose a KDP trim size.");
        RuleFor(c => c.Style).IsInEnum();
        RuleFor(c => c.PageCount)
            .InclusiveBetween(KdpPrintRules.MinPageCount, KdpPrintRules.MaxPageCount(PaperType.BlackInkWhitePaper, TrimSize.All[0]));
    }
}

internal sealed class GenerateJournalCommandHandler(IInteriorRenderer renderer)
    : ICommandHandler<GenerateJournalCommand, GeneratedFile>
{
    public Task<Result<GeneratedFile>> Handle(GenerateJournalCommand command, CancellationToken ct)
    {
        var trim = TrimSize.Find(command.TrimSizeId)!;
        var pageCheck = PageCountRule.Validate(trim, command.PageCount, PaperType.BlackInkWhitePaper);
        if (pageCheck.IsFailure)
        {
            return Task.FromResult(Result.Failure<GeneratedFile>(pageCheck.Error));
        }

        var journal = new Journal(command.Title, command.Author, command.Style, command.PageCount);
        var file = renderer.RenderJournal(journal, trim, BookPagePlan.For(trim, command.PageCount));
        return Task.FromResult(Result.Success(file));
    }
}
