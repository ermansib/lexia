// Shapes match the .NET DTOs exactly. Change both sides together.

export type BookType =
  | 'ColoringBook'
  | 'ColorByNumber'
  | 'WordSearch'
  | 'Sudoku'
  | 'Crossword'
  | 'Maze'
  | 'Journal'
  | 'Planner'
  | 'Logbook'
  | 'KidsStory'
  | 'NonFiction'
  | 'Fiction';

export type BookCategory = 'LowContent' | 'Illustrated' | 'Written';

export type BookStage = 'Idea' | 'Research' | 'Create' | 'Design' | 'Publish' | 'Published';

export type PaperType =
  | 'BlackInkWhitePaper'
  | 'BlackInkCreamPaper'
  | 'StandardColorWhitePaper'
  | 'PremiumColorWhitePaper';

export interface BookDto {
  id: string;
  title: string;
  subtitle: string | null;
  authorName: string | null;
  type: BookType;
  category: BookCategory;
  stage: BookStage;
  seriesName: string | null;
  seriesNumber: number | null;
  trimSizeId: string | null;
  pageCount: number | null;
  paper: PaperType | null;
  description: string | null;
  keywords: string[];
  isReadyToPublish: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  total: number;
}

export interface TrimSizeDto {
  id: string;
  widthIn: number;
  heightIn: number;
  label: string;
  isLarge: boolean;
}

export interface CoverDimensionsDto {
  trimSizeId: string;
  pageCount: number;
  paper: PaperType;
  spineWidthIn: number;
  fullWidthIn: number;
  fullHeightIn: number;
  bleedIn: number;
  safeZoneIn: number;
  spineTextAllowed: boolean;
  requiredDpi: number;
  fullWidthPx: number;
  fullHeightPx: number;
}

export type IssueSeverity = 'Warning' | 'Error';

export interface PrintIssueDto {
  code: string;
  severity: IssueSeverity;
  message: string;
  fix: string;
}

export interface PrintReadinessReportDto {
  isReady: boolean;
  errorCount: number;
  warningCount: number;
  issues: PrintIssueDto[];
}

export interface CheckPrintReadinessRequest {
  trimSizeId: string;
  paper: PaperType;
  pageCount: number;
  hasBleed: boolean;
  pageWidthIn: number;
  pageHeightIn: number;
  insideMarginIn: number;
  outsideMarginIn: number;
  topMarginIn: number;
  bottomMarginIn: number;
  lowestImageDpi: number | null;
  allFontsEmbedded: boolean;
  hasSpineText: boolean;
}

export type PuzzleType = 'WordSearch' | 'Sudoku' | 'Maze';
export type Difficulty = 'Easy' | 'Medium' | 'Hard';
export type JournalStyle = 'Lined' | 'Dotted' | 'Grid' | 'Blank';

export interface WordListInput {
  title: string;
  words: string[];
}

export interface GeneratePuzzleBookRequest {
  title: string;
  author: string | null;
  trimSizeId: string;
  puzzleType: PuzzleType;
  difficulty: Difficulty;
  puzzleCount: number;
  wordLists: WordListInput[] | null;
  includeSolutions: boolean;
  seed: number | null;
}

export interface GenerateJournalRequest {
  title: string;
  author: string | null;
  trimSizeId: string;
  style: JournalStyle;
  pageCount: number;
}

/** RFC 7807 problem details returned by every Lexia service. */
export interface ProblemDetails {
  title?: string;
  detail?: string;
  status?: number;
  code?: string;
  errors?: Record<string, string[]>;
}
