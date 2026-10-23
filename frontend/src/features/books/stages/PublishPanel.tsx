import { useState, type FormEvent } from 'react';
import { Button } from '@/components/ui/Button';
import { Card } from '@/components/ui/Card';
import { Badge, ErrorMessage } from '@/components/ui/Feedback';
import { TextAreaField } from '@/components/ui/Field';
import { KDP_MAX_KEYWORD_LENGTH, KDP_MAX_KEYWORDS } from '@/constants/labels';
import { booksApi } from '@/services/api/booksApi';
import type { BookDto } from '@/services/api/types';
import { useBookMutation } from '../useBooks';

const MAX_DESCRIPTION = 4000;

const padKeywords = (keywords: string[]) =>
  [...keywords, ...Array<string>(KDP_MAX_KEYWORDS).fill('')].slice(0, KDP_MAX_KEYWORDS);

export const PublishPanel = ({ book }: { book: BookDto }) => {
  const [description, setDescription] = useState(book.description ?? '');
  const [keywords, setKeywords] = useState(padKeywords(book.keywords));
  const save = useBookMutation((body: Parameters<typeof booksApi.setListing>[1]) =>
    booksApi.setListing(book.id, body),
  );

  const submit = (event: FormEvent) => {
    event.preventDefault();
    save.mutate({ description: description || null, keywords: keywords.filter((k) => k.trim()) });
  };

  const checklist = [
    {
      done: Boolean(book.trimSizeId && book.pageCount && book.paper),
      label: 'Print spec saved (Design step)',
    },
    { done: Boolean(book.description), label: 'Description written' },
    { done: book.keywords.length > 0, label: 'At least one keyword' },
  ];

  return (
    <div className="space-y-6">
      <Card
        title="Amazon listing"
        description="The description and the 7 backend keyword boxes you paste into KDP."
      >
        <form onSubmit={submit} className="space-y-4">
          <TextAreaField
            label="Description"
            rows={6}
            maxLength={MAX_DESCRIPTION}
            hint={`${description.length} / ${MAX_DESCRIPTION} characters`}
            value={description}
            onChange={(e) => setDescription(e.target.value)}
          />
          <fieldset>
            <legend className="text-sm font-medium text-slate-700">Backend keywords</legend>
            <div className="mt-1 grid gap-2 sm:grid-cols-2">
              {keywords.map((keyword, index) => (
                <input
                  key={index}
                  aria-label={`Keyword ${index + 1}`}
                  maxLength={KDP_MAX_KEYWORD_LENGTH}
                  placeholder={`Keyword phrase ${index + 1}`}
                  value={keyword}
                  onChange={(e) =>
                    setKeywords((all) => all.map((k, i) => (i === index ? e.target.value : k)))
                  }
                  className="rounded-lg border-0 px-3 py-2 text-sm ring-1 ring-slate-300 focus:ring-2 focus:ring-indigo-600"
                />
              ))}
            </div>
          </fieldset>
          <Button type="submit" busy={save.isPending}>
            Save listing
          </Button>
          <ErrorMessage error={save.error} />
        </form>
      </Card>

      <Card title="Ready to go live?">
        <ul className="space-y-2 text-sm">
          {checklist.map((item) => (
            <li key={item.label} className="flex items-center gap-2">
              <Badge tone={item.done ? 'success' : 'warning'}>{item.done ? 'Done' : 'To do'}</Badge>
              {item.label}
            </li>
          ))}
        </ul>
        <p className="mt-4 text-sm text-slate-600">
          Upload your interior and cover on KDP, then mark the book live with the button above.
        </p>
      </Card>
    </div>
  );
};
