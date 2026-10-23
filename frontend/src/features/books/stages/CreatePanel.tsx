import { Card } from '@/components/ui/Card';
import { Badge } from '@/components/ui/Feedback';
import { JournalMaker } from '@/features/tools/JournalMaker';
import { PuzzleBookMaker } from '@/features/tools/PuzzleBookMaker';
import type { BookDto, BookType, PuzzleType } from '@/services/api/types';

const PUZZLE_TYPES: Partial<Record<BookType, PuzzleType>> = {
  WordSearch: 'WordSearch',
  Sudoku: 'Sudoku',
  Maze: 'Maze',
};

const JOURNAL_TYPES: BookType[] = ['Journal', 'Planner', 'Logbook'];

/** Shows the right creation tool for the book type. */
export const CreatePanel = ({ book }: { book: BookDto }) => {
  const puzzleType = PUZZLE_TYPES[book.type];
  if (puzzleType) {
    return (
      <PuzzleBookMaker
        initialTitle={book.title}
        initialAuthor={book.authorName}
        initialType={puzzleType}
        initialTrimSizeId={book.trimSizeId}
      />
    );
  }

  if (JOURNAL_TYPES.includes(book.type)) {
    return (
      <JournalMaker
        initialTitle={book.title}
        initialAuthor={book.authorName}
        initialTrimSizeId={book.trimSizeId}
      />
    );
  }

  return (
    <Card
      title="Create the content"
      description={
        book.category === 'Written'
          ? 'The AI co-writer (outline, then chapter by chapter, with a story bible for characters and series) is being built next.'
          : 'AI illustration with consistent characters and a locked art style is being built next.'
      }
      actions={<Badge tone="info">Coming soon</Badge>}
    >
      <p className="text-sm text-slate-700">
        You can still move on to Design and Publish to set the print spec, cover size and listing for this
        book.
      </p>
    </Card>
  );
};
