using System.Globalization;
using Lexia.Studio.Domain.Puzzles;
using PdfSharp.Drawing;

namespace Lexia.Studio.Infrastructure.Pdf;

/// <summary>Draws puzzles and their solutions inside a page's content box.</summary>
internal static class PuzzleDrawing
{
    private const double HeadingGap = 14;
    private const int WordListColumns = 3;
    private static readonly XPen Highlight = new(XColor.FromArgb(110, 120, 120, 120), 9) { LineCap = XLineCap.Round };
    private static readonly XPen SolutionPath = new(XColor.FromGrayScale(0.35), 2.5) { LineCap = XLineCap.Round, LineJoin = XLineJoin.Round };
    private static readonly XBrush SolvedDigit = new XSolidBrush(XColor.FromGrayScale(0.45));

    public static void WordSearch(XGraphics gfx, XRect content, WordSearch puzzle, int number, bool showSolution)
    {
        var y = Heading(gfx, content, number, puzzle.Title, showSolution);
        var wordRows = (int)Math.Ceiling(puzzle.Placements.Count / (double)WordListColumns);
        var wordListHeight = wordRows * 14 + HeadingGap;
        var side = Math.Min(content.Width, content.Bottom - y - wordListHeight);
        var cell = side / puzzle.Size;
        var left = content.X + (content.Width - side) / 2;
        var font = Typography.Mono(cell * 0.55);

        if (showSolution)
        {
            foreach (var p in puzzle.Placements)
            {
                var endRow = p.Row + (p.Word.Length - 1) * p.RowStep;
                var endColumn = p.Column + (p.Word.Length - 1) * p.ColumnStep;
                gfx.DrawLine(Highlight,
                    left + (p.Column + 0.5) * cell, y + (p.Row + 0.5) * cell,
                    left + (endColumn + 0.5) * cell, y + (endRow + 0.5) * cell);
            }
        }

        for (var r = 0; r < puzzle.Size; r++)
        {
            for (var c = 0; c < puzzle.Size; c++)
            {
                var box = new XRect(left + c * cell, y + r * cell, cell, cell);
                gfx.DrawString(puzzle.Grid[r, c].ToString(), font, XBrushes.Black, box, XStringFormats.Center);
            }
        }

        gfx.DrawRectangle(Typography.Rule, left, y, side, side);
        if (!showSolution)
        {
            WordList(gfx, content, puzzle, y + side + HeadingGap);
        }
    }

    public static void SudokuPage(XGraphics gfx, XRect content, Sudoku sudoku, int number)
    {
        var y = Heading(gfx, content, number, $"Sudoku ({sudoku.Difficulty})", solution: false);
        var side = Math.Min(content.Width, content.Bottom - y);
        SudokuGrid(gfx, content.X + (content.Width - side) / 2, y, side, sudoku, showSolution: false);
    }

    public static void SudokuSolutions(XGraphics gfx, XRect content, IEnumerable<(Sudoku Sudoku, int Number)> solutions)
    {
        const double gap = 18;
        var side = Math.Min((content.Width - gap) / 2, (content.Height - gap) / 2 - HeadingGap);
        var index = 0;
        foreach (var (sudoku, number) in solutions)
        {
            var x = content.X + index % 2 * (side + gap);
            var y = content.Y + index / 2 * (side + gap + HeadingGap);
            gfx.DrawString($"Puzzle {number}", Typography.Small, XBrushes.Black, new XRect(x, y, side, HeadingGap), XStringFormats.TopLeft);
            SudokuGrid(gfx, x, y + HeadingGap, side, sudoku, showSolution: true);
            index++;
        }
    }

    public static void Maze(XGraphics gfx, XRect content, Maze maze, int number, bool showSolution)
    {
        var y = Heading(gfx, content, number, "Maze", showSolution);
        var cell = Math.Min(content.Width / maze.Columns, (content.Bottom - y) / maze.Rows);
        var left = content.X + (content.Width - cell * maze.Columns) / 2;

        for (var r = 0; r < maze.Rows; r++)
        {
            for (var c = 0; c < maze.Columns; c++)
            {
                var walls = maze.Cells[r, c];
                var x = left + c * cell;
                var top = y + r * cell;
                var isEntrance = r == 0 && c == 0;
                var isExit = r == maze.Rows - 1 && c == maze.Columns - 1;

                if (walls.HasFlag(Walls.North) && !isEntrance)
                {
                    gfx.DrawLine(Typography.Rule, x, top, x + cell, top);
                }

                if (walls.HasFlag(Walls.West))
                {
                    gfx.DrawLine(Typography.Rule, x, top, x, top + cell);
                }

                if (r == maze.Rows - 1 && walls.HasFlag(Walls.South) && !isExit)
                {
                    gfx.DrawLine(Typography.Rule, x, top + cell, x + cell, top + cell);
                }

                if (c == maze.Columns - 1 && walls.HasFlag(Walls.East))
                {
                    gfx.DrawLine(Typography.Rule, x + cell, top, x + cell, top + cell);
                }
            }
        }

        if (showSolution)
        {
            var points = maze.Solution
                .Select(p => new XPoint(left + (p.Column + 0.5) * cell, y + (p.Row + 0.5) * cell))
                .ToArray();
            gfx.DrawLines(SolutionPath, points);
        }
    }

    private static double Heading(XGraphics gfx, XRect content, int number, string title, bool solution)
    {
        var label = solution ? $"Solution {number}: {title}" : $"{number}. {title}";
        return Typography.DrawCentered(gfx, label, Typography.Heading, content, content.Y) + HeadingGap;
    }

    private static void WordList(XGraphics gfx, XRect content, WordSearch puzzle, double y)
    {
        var columnWidth = content.Width / WordListColumns;
        var words = puzzle.Placements.Select(p => p.Word).Order(StringComparer.Ordinal).ToList();
        var rows = (int)Math.Ceiling(words.Count / (double)WordListColumns);
        for (var i = 0; i < words.Count; i++)
        {
            var column = i / rows;
            var row = i % rows;
            var box = new XRect(content.X + column * columnWidth, y + row * 14, columnWidth, 14);
            gfx.DrawString(words[i], Typography.Body, XBrushes.Black, box, XStringFormats.TopCenter);
        }
    }

    private static void SudokuGrid(XGraphics gfx, double left, double top, double side, Sudoku sudoku, bool showSolution)
    {
        var cell = side / Sudoku.Size;
        var font = Typography.Sans(cell * 0.55, bold: true);
        var solvedFont = Typography.Sans(cell * 0.55);

        for (var i = 0; i <= Sudoku.Size; i++)
        {
            var pen = i % 3 == 0 ? Typography.Thick : Typography.Hairline;
            gfx.DrawLine(pen, left + i * cell, top, left + i * cell, top + side);
            gfx.DrawLine(pen, left, top + i * cell, left + side, top + i * cell);
        }

        for (var index = 0; index < Sudoku.Size * Sudoku.Size; index++)
        {
            var given = sudoku.Puzzle[index];
            if (given == 0 && !showSolution)
            {
                continue;
            }

            var box = new XRect(left + index % Sudoku.Size * cell, top + index / Sudoku.Size * cell, cell, cell);
            var value = (given == 0 ? sudoku.Solution[index] : given).ToString(CultureInfo.InvariantCulture);
            gfx.DrawString(value, given == 0 ? solvedFont : font, given == 0 ? SolvedDigit : XBrushes.Black, box, XStringFormats.Center);
        }
    }
}
