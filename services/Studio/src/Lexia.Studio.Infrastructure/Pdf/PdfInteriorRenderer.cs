using System.Globalization;
using Lexia.Studio.Application.Abstractions;
using Lexia.Studio.Application.PuzzleBooks;
using Lexia.Studio.Domain.Kdp;
using Lexia.Studio.Domain.Puzzles;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Lexia.Studio.Infrastructure.Pdf;

/// <summary>Renders print-ready KDP interiors with PDFsharp (MIT). Fonts are always embedded.</summary>
internal sealed class PdfInteriorRenderer(TimeProvider clock) : IInteriorRenderer
{
    private const string PdfContentType = "application/pdf";
    private const double HeadingGap = 18;

    public GeneratedFile RenderPuzzleBook(PuzzleBook book, TrimSize trim, BookPagePlan plan)
    {
        var (document, frame) = Start(book.Title, book.Author, plan.Layout);
        DrawTitlePage(frame, book.Title, book.Author);
        DrawInstructionsPage(frame, book.Author, Instructions(book.Type));

        for (var i = 0; i < book.PuzzleCount; i++)
        {
            var (gfx, content) = frame.AddPage();
            using (gfx)
            {
                var number = i + 1;
                switch (book.Type)
                {
                    case PuzzleType.WordSearch:
                        PuzzleDrawing.WordSearch(gfx, content, book.WordSearches[i], number, showSolution: false);
                        break;
                    case PuzzleType.Sudoku:
                        PuzzleDrawing.SudokuPage(gfx, content, book.Sudokus[i], number);
                        break;
                    default:
                        PuzzleDrawing.Maze(gfx, content, book.Mazes[i], number, showSolution: false);
                        break;
                }
            }
        }

        if (book.IncludeSolutions)
        {
            DrawSolutions(frame, book);
        }

        PadWithNotes(frame, plan.TotalPages);
        return Finish(document, book.Title, frame.PageCount);
    }

    public GeneratedFile RenderJournal(Journal journal, TrimSize trim, BookPagePlan plan)
    {
        var (document, frame) = Start(journal.Title, journal.Author, plan.Layout);
        DrawTitlePage(frame, journal.Title, journal.Author);

        while (frame.PageCount < plan.TotalPages)
        {
            var (gfx, content) = frame.AddPage();
            using (gfx)
            {
                JournalDrawing.Page(gfx, content, journal.Style);
            }
        }

        return Finish(document, journal.Title, frame.PageCount);
    }

    private static (PdfDocument Document, PageFrame Frame) Start(string title, string? author, InteriorLayout layout)
    {
        EmbeddedFontResolver.EnsureInstalled();
        var document = new PdfDocument();
        document.Info.Title = title;
        document.Info.Author = author ?? string.Empty;
        document.Info.Creator = "Lexia";
        return (document, new PageFrame(document, layout));
    }

    private static GeneratedFile Finish(PdfDocument document, string title, int pageCount)
    {
        using var stream = new MemoryStream();
        document.Save(stream);
        document.Dispose();
        return new GeneratedFile($"{FileNames.Slug(title)}-interior.pdf", PdfContentType, stream.ToArray(), pageCount);
    }

    private static void DrawTitlePage(PageFrame frame, string title, string? author)
    {
        var (gfx, content) = frame.AddPage();
        using (gfx)
        {
            var y = content.Y + content.Height * 0.3;
            y = Typography.DrawCentered(gfx, title, Typography.Title, content, y) + HeadingGap;
            if (!string.IsNullOrWhiteSpace(author))
            {
                Typography.DrawCentered(gfx, author, Typography.Subtitle, content, y);
            }
        }
    }

    private void DrawInstructionsPage(PageFrame frame, string? author, string instructions)
    {
        var (gfx, content) = frame.AddPage();
        using (gfx)
        {
            var y = Typography.DrawCentered(gfx, "How to play", Typography.Heading, content, content.Y) + HeadingGap;
            y = Typography.DrawWrapped(gfx, instructions, Typography.Body, content, y) + HeadingGap * 2;
            var year = clock.GetUtcNow().Year.ToString(CultureInfo.InvariantCulture);
            var owner = string.IsNullOrWhiteSpace(author) ? "the author" : author;
            Typography.DrawWrapped(gfx, $"Copyright © {year} {owner}. All rights reserved.", Typography.Small, content, y);
        }
    }

    private static void DrawSolutions(PageFrame frame, PuzzleBook book)
    {
        var (divider, dividerContent) = frame.AddPage();
        using (divider)
        {
            Typography.DrawCentered(divider, "Solutions", Typography.Title, dividerContent, dividerContent.Y + dividerContent.Height * 0.4);
        }

        if (book.Type == PuzzleType.Sudoku)
        {
            foreach (var chunk in book.Sudokus.Select((s, i) => (Sudoku: s, Number: i + 1)).Chunk(PuzzleBookPages.SudokuSolutionsPerPage))
            {
                var (gfx, content) = frame.AddPage();
                using (gfx)
                {
                    PuzzleDrawing.SudokuSolutions(gfx, content, chunk);
                }
            }

            return;
        }

        for (var i = 0; i < book.PuzzleCount; i++)
        {
            var (gfx, content) = frame.AddPage();
            using (gfx)
            {
                if (book.Type == PuzzleType.WordSearch)
                {
                    PuzzleDrawing.WordSearch(gfx, content, book.WordSearches[i], i + 1, showSolution: true);
                }
                else
                {
                    PuzzleDrawing.Maze(gfx, content, book.Mazes[i], i + 1, showSolution: true);
                }
            }
        }
    }

    private static void PadWithNotes(PageFrame frame, int totalPages)
    {
        while (frame.PageCount < totalPages)
        {
            var (gfx, content) = frame.AddPage();
            using (gfx)
            {
                var y = Typography.DrawCentered(gfx, "Notes", Typography.Heading, content, content.Y) + HeadingGap;
                JournalDrawing.Lines(gfx, new XRect(content.X, y, content.Width, content.Bottom - y));
            }
        }
    }

    private static string Instructions(PuzzleType type) => type switch
    {
        PuzzleType.WordSearch => "Find every word from the list hidden in the grid. Words can run across, down or diagonally, and on harder puzzles they can run backwards. Circle each word as you find it. Solutions are at the back of the book.",
        PuzzleType.Sudoku => "Fill every empty square with a number from 1 to 9 so that each row, each column and each 3x3 box contains every number exactly once. Every puzzle has exactly one solution, found at the back of the book.",
        _ => "Start at the arrow at the top left and find your way to the exit at the bottom right without crossing any walls. Solutions are at the back of the book.",
    };
}

internal static class FileNames
{
    private const int MaxSlugLength = 60;

    public static string Slug(string title)
    {
        var slug = new string([.. title.ToLowerInvariant().Select(c => char.IsAsciiLetterOrDigit(c) ? c : '-')]);
        while (slug.Contains("--", StringComparison.Ordinal))
        {
            slug = slug.Replace("--", "-", StringComparison.Ordinal);
        }

        slug = slug.Trim('-');
        if (slug.Length > MaxSlugLength)
        {
            slug = slug[..MaxSlugLength].TrimEnd('-');
        }

        return slug.Length == 0 ? "book" : slug;
    }
}
