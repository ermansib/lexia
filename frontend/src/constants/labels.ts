import type {
  BookCategory,
  BookStage,
  BookType,
  Difficulty,
  JournalStyle,
  PaperType,
  PuzzleType,
} from '@/services/api/types';

export const BOOK_TYPE_LABELS: Record<BookType, string> = {
  ColoringBook: 'Coloring book',
  ColorByNumber: 'Color by number',
  WordSearch: 'Word search',
  Sudoku: 'Sudoku',
  Crossword: 'Crossword',
  Maze: 'Maze book',
  Journal: 'Journal',
  Planner: 'Planner',
  Logbook: 'Logbook',
  KidsStory: "Kids' story",
  NonFiction: 'Non-fiction',
  Fiction: 'Fiction',
};

export const BOOK_CATEGORY_LABELS: Record<BookCategory, string> = {
  LowContent: 'Low-content',
  Illustrated: 'Illustrated',
  Written: 'Written',
};

export const STAGES: BookStage[] = ['Idea', 'Research', 'Create', 'Design', 'Publish', 'Published'];

export const STAGE_LABELS: Record<BookStage, string> = {
  Idea: 'Idea',
  Research: 'Research',
  Create: 'Create',
  Design: 'Design',
  Publish: 'Publish',
  Published: 'Live',
};

export const PAPER_LABELS: Record<PaperType, string> = {
  BlackInkWhitePaper: 'Black ink, white paper',
  BlackInkCreamPaper: 'Black ink, cream paper',
  StandardColorWhitePaper: 'Standard color, white paper',
  PremiumColorWhitePaper: 'Premium color, white paper',
};

export const PUZZLE_TYPE_LABELS: Record<PuzzleType, string> = {
  WordSearch: 'Word search',
  Sudoku: 'Sudoku',
  Maze: 'Mazes',
};

export const DIFFICULTIES: Difficulty[] = ['Easy', 'Medium', 'Hard'];

export const JOURNAL_STYLE_LABELS: Record<JournalStyle, string> = {
  Lined: 'Lined',
  Dotted: 'Dot grid',
  Grid: 'Graph grid',
  Blank: 'Blank',
};

export const KDP_MAX_KEYWORDS = 7;
export const KDP_MAX_KEYWORD_LENGTH = 50;
export const KDP_MIN_PAGES = 24;
