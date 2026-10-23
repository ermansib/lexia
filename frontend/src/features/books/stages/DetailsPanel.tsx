import { useState, type FormEvent } from 'react';
import { Button } from '@/components/ui/Button';
import { Card } from '@/components/ui/Card';
import { ErrorMessage } from '@/components/ui/Feedback';
import { TextField } from '@/components/ui/Field';
import { booksApi } from '@/services/api/booksApi';
import type { BookDto } from '@/services/api/types';
import { useBookMutation } from '../useBooks';

export const DetailsPanel = ({ book }: { book: BookDto }) => {
  const [title, setTitle] = useState(book.title);
  const [subtitle, setSubtitle] = useState(book.subtitle ?? '');
  const [authorName, setAuthorName] = useState(book.authorName ?? '');
  const [seriesName, setSeriesName] = useState(book.seriesName ?? '');
  const [seriesNumber, setSeriesNumber] = useState(book.seriesNumber?.toString() ?? '');
  const save = useBookMutation((body: Parameters<typeof booksApi.updateDetails>[1]) =>
    booksApi.updateDetails(book.id, body),
  );

  const submit = (event: FormEvent) => {
    event.preventDefault();
    save.mutate({
      title,
      subtitle: subtitle || null,
      authorName: authorName || null,
      seriesName: seriesName || null,
      seriesNumber: seriesNumber ? Number(seriesNumber) : null,
    });
  };

  return (
    <Card title="Shape the idea" description="Name the book and decide whether it belongs to a series.">
      <form onSubmit={submit} className="grid gap-4 sm:grid-cols-2">
        <TextField label="Title" value={title} onChange={(e) => setTitle(e.target.value)} required />
        <TextField label="Subtitle" value={subtitle} onChange={(e) => setSubtitle(e.target.value)} />
        <TextField
          label="Author or pen name"
          value={authorName}
          onChange={(e) => setAuthorName(e.target.value)}
        />
        <div className="grid grid-cols-[2fr_1fr] gap-3">
          <TextField label="Series" value={seriesName} onChange={(e) => setSeriesName(e.target.value)} />
          <TextField
            label="Number"
            type="number"
            min={1}
            value={seriesNumber}
            onChange={(e) => setSeriesNumber(e.target.value)}
          />
        </div>
        <div className="flex items-center gap-3 sm:col-span-2">
          <Button type="submit" busy={save.isPending}>
            Save details
          </Button>
          {save.isSuccess && <span className="text-sm text-emerald-700">Saved.</span>}
        </div>
        <div className="sm:col-span-2">
          <ErrorMessage error={save.error} />
        </div>
      </form>
    </Card>
  );
};
