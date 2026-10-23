import { useState, type FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button } from '@/components/ui/Button';
import { ErrorMessage } from '@/components/ui/Feedback';
import { SelectField, TextField } from '@/components/ui/Field';
import { toOptions } from '@/components/ui/options';
import { BOOK_TYPE_LABELS } from '@/constants/labels';
import { useBookMutation } from '@/features/books/useBooks';
import { booksApi } from '@/services/api/booksApi';
import type { BookType } from '@/services/api/types';

export const NewBookForm = () => {
  const navigate = useNavigate();
  const [title, setTitle] = useState('');
  const [type, setType] = useState<BookType>('ColoringBook');
  const create = useBookMutation(booksApi.create);

  const submit = (event: FormEvent) => {
    event.preventDefault();
    create.mutate({ title, type, authorName: null }, { onSuccess: (book) => navigate(`/books/${book.id}`) });
  };

  return (
    <form onSubmit={submit} className="grid gap-4 sm:grid-cols-[2fr_1fr_auto] sm:items-end">
      <TextField
        label="Working title"
        placeholder="e.g. Ocean Animals Coloring Book for Kids"
        value={title}
        onChange={(e) => setTitle(e.target.value)}
        required
      />
      <SelectField
        label="Book type"
        value={type}
        onValueChange={setType}
        options={toOptions(BOOK_TYPE_LABELS)}
      />
      <Button type="submit" busy={create.isPending}>
        Start book
      </Button>
      <div className="sm:col-span-3">
        <ErrorMessage error={create.error} />
      </div>
    </form>
  );
};
