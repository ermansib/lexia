import { useState, type ReactNode } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Button } from '@/components/ui/Button';
import { Badge, ErrorMessage } from '@/components/ui/Feedback';
import { BOOK_CATEGORY_LABELS, BOOK_TYPE_LABELS, STAGE_LABELS, STAGES } from '@/constants/labels';
import { booksApi } from '@/services/api/booksApi';
import type { BookDto, BookStage } from '@/services/api/types';
import { StageStepper } from './StageStepper';
import { CreatePanel } from './stages/CreatePanel';
import { DesignPanel } from './stages/DesignPanel';
import { DetailsPanel } from './stages/DetailsPanel';
import { PublishPanel } from './stages/PublishPanel';
import { ResearchPanel } from './stages/ResearchPanel';
import { useBook, useBookMutation, useDeleteBook } from './useBooks';

const PANELS: Record<BookStage, (book: BookDto) => ReactNode> = {
  Idea: (book) => <DetailsPanel book={book} />,
  Research: () => <ResearchPanel />,
  Create: (book) => <CreatePanel book={book} />,
  Design: (book) => <DesignPanel book={book} />,
  Publish: (book) => <PublishPanel book={book} />,
  Published: (book) => <PublishPanel book={book} />,
};

export const BookPage = () => {
  const { bookId = '' } = useParams();
  const book = useBook(bookId);

  if (book.error) return <ErrorMessage error={book.error} />;
  if (!book.data) return <p className="text-slate-500">Loading…</p>;
  return <BookWorkspace key={book.data.id} book={book.data} />;
};

const BookWorkspace = ({ book }: { book: BookDto }) => {
  const navigate = useNavigate();
  const [viewing, setViewing] = useState<BookStage>(book.stage);
  const move = useBookMutation((stage: BookStage) => booksApi.moveToStage(book.id, stage));
  const remove = useDeleteBook();

  const nextStage = STAGES[STAGES.indexOf(book.stage) + 1] as BookStage | undefined;
  const advance = () => {
    if (!nextStage) return;
    move.mutate(nextStage, { onSuccess: (updated) => setViewing(updated.stage) });
  };

  const deleteBook = () => {
    if (window.confirm(`Delete "${book.title}"? This cannot be undone.`)) {
      remove.mutate(book.id, { onSuccess: () => navigate('/') });
    }
  };

  return (
    <div className="space-y-6">
      <header className="flex flex-wrap items-start justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-slate-900">{book.title}</h1>
          <div className="mt-2 flex flex-wrap gap-2">
            <Badge>{BOOK_TYPE_LABELS[book.type]}</Badge>
            <Badge tone="info">{BOOK_CATEGORY_LABELS[book.category]}</Badge>
            {book.seriesName && (
              <Badge>{`${book.seriesName}${book.seriesNumber ? ` #${book.seriesNumber}` : ''}`}</Badge>
            )}
          </div>
        </div>
        <div className="flex gap-2">
          <Button variant="danger" onClick={deleteBook} busy={remove.isPending}>
            Delete
          </Button>
          {nextStage && (
            <Button onClick={advance} busy={move.isPending}>
              {nextStage === 'Published' ? 'Mark as live' : `Next: ${STAGE_LABELS[nextStage]}`}
            </Button>
          )}
        </div>
      </header>

      <StageStepper current={book.stage} viewing={viewing} onView={setViewing} />
      <ErrorMessage error={move.error ?? remove.error} />
      {PANELS[viewing](book)}
    </div>
  );
};
