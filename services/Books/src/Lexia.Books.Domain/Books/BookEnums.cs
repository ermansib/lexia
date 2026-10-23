namespace Lexia.Books.Domain.Books;

/// <summary>Every kind of book Lexia can help create.</summary>
public enum BookType
{
    ColoringBook,
    ColorByNumber,
    WordSearch,
    Sudoku,
    Crossword,
    Maze,
    Journal,
    Planner,
    Logbook,
    KidsStory,
    NonFiction,
    Fiction,
}

/// <summary>Groups book types by how they are made, which decides the tools in the guided flow.</summary>
public enum BookCategory
{
    LowContent,
    Illustrated,
    Written,
}

/// <summary>The steps of Lexia's guided flow, from idea to a live book.</summary>
public enum BookStage
{
    Idea,
    Research,
    Create,
    Design,
    Publish,
    Published,
}

public enum PaperType
{
    BlackInkWhitePaper,
    BlackInkCreamPaper,
    StandardColorWhitePaper,
    PremiumColorWhitePaper,
}

public static class BookTypeExtensions
{
    public static BookCategory Category(this BookType type) => type switch
    {
        BookType.ColoringBook or BookType.ColorByNumber or BookType.KidsStory => BookCategory.Illustrated,
        BookType.NonFiction or BookType.Fiction => BookCategory.Written,
        _ => BookCategory.LowContent,
    };
}
