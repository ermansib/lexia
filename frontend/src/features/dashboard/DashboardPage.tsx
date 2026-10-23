import { useState } from 'react';
import { Link } from 'react-router-dom';
import { Card } from '@/components/ui/Card';
import { Badge, EmptyState, ErrorMessage } from '@/components/ui/Feedback';
import { BOOK_TYPE_LABELS, STAGE_LABELS, STAGES } from '@/constants/labels';
import { useBookList } from '@/features/books/useBooks';
import type { BookDto, BookStage } from '@/services/api/types';
import { NewBookForm } from './NewBookForm';

export const DashboardPage = () => {
  const [stage, setStage] = useState<BookStage | undefined>();
  const [search, setSearch] = useState('');
  const books = useBookList({ stage, search });

  return (
    <div className="space-y-6">
      <header>
        <h1 className="text-2xl font-bold text-slate-900">Your books</h1>
        <p className="mt-1 text-slate-600">
          Every book moves through the same guided flow: idea, research, create, design, publish.
        </p>
      </header>

      <Card title="Start a new book">
        <NewBookForm />
      </Card>

      <div className="flex flex-wrap items-center gap-2">
        <FilterChip active={stage === undefined} onClick={() => setStage(undefined)}>
          All
        </FilterChip>
        {STAGES.map((s) => (
          <FilterChip key={s} active={stage === s} onClick={() => setStage(s)}>
            {STAGE_LABELS[s]}
          </FilterChip>
        ))}
        <input
          type="search"
          aria-label="Search books"
          placeholder="Search titles or series"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="ml-auto rounded-lg border-0 px-3 py-1.5 text-sm ring-1 ring-slate-300 focus:ring-2 focus:ring-indigo-600"
        />
      </div>

      <ErrorMessage error={books.error} />
      {books.data && books.data.items.length === 0 && (
        <EmptyState title="No books here yet">
          Start one above, or try a free tool from the sidebar.
        </EmptyState>
      )}
      <ul className="grid gap-4 sm:grid-cols-2">
        {books.data?.items.map((book) => <BookCard key={book.id} book={book} />)}
      </ul>
    </div>
  );
};

const FilterChip = ({
  active,
  onClick,
  children,
}: {
  active: boolean;
  onClick: () => void;
  children: string;
}) => (
  <button
    type="button"
    aria-pressed={active}
    onClick={onClick}
    className={`rounded-full px-3 py-1 text-sm ${active ? 'bg-indigo-600 text-white' : 'bg-white text-slate-700 ring-1 ring-slate-300 hover:bg-slate-50'}`}
  >
    {children}
  </button>
);

const BookCard = ({ book }: { book: BookDto }) => (
  <li>
    <Link
      to={`/books/${book.id}`}
      className="block rounded-2xl bg-white p-5 shadow-sm ring-1 ring-slate-200 transition hover:ring-indigo-300"
    >
      <div className="flex items-start justify-between gap-3">
        <h3 className="font-semibold text-slate-900">{book.title}</h3>
        <Badge tone={book.stage === 'Published' ? 'success' : 'info'}>{STAGE_LABELS[book.stage]}</Badge>
      </div>
      <p className="mt-1 text-sm text-slate-600">
        {BOOK_TYPE_LABELS[book.type]}
        {book.seriesName && ` · ${book.seriesName}${book.seriesNumber ? ` #${book.seriesNumber}` : ''}`}
      </p>
      <p className="mt-3 text-xs text-slate-400">Updated {new Date(book.updatedAt).toLocaleDateString()}</p>
    </Link>
  </li>
);
