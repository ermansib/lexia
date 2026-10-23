import { useState, type FormEvent } from 'react';
import { Button } from '@/components/ui/Button';
import { Card } from '@/components/ui/Card';
import { ErrorMessage } from '@/components/ui/Feedback';
import { SelectField, TextField } from '@/components/ui/Field';
import { toOptions } from '@/components/ui/options';
import { KDP_MIN_PAGES, PAPER_LABELS } from '@/constants/labels';
import { CoverCalculator } from '@/features/tools/CoverCalculator';
import { useTrimSizes } from '@/hooks/useTrimSizes';
import { booksApi } from '@/services/api/booksApi';
import type { BookDto, PaperType } from '@/services/api/types';
import { useBookMutation } from '../useBooks';

const DEFAULT_PAGES = 120;

export const DesignPanel = ({ book }: { book: BookDto }) => {
  const trimSizes = useTrimSizes();
  const [trimSizeId, setTrimSizeId] = useState(book.trimSizeId ?? '6x9');
  const [pageCount, setPageCount] = useState(book.pageCount ?? DEFAULT_PAGES);
  const [paper, setPaper] = useState<PaperType>(book.paper ?? 'BlackInkWhitePaper');
  const save = useBookMutation((body: Parameters<typeof booksApi.setPrintSpec>[1]) =>
    booksApi.setPrintSpec(book.id, body),
  );

  const submit = (event: FormEvent) => {
    event.preventDefault();
    save.mutate({ trimSizeId, pageCount, paper });
  };

  return (
    <div className="space-y-6">
      <Card
        title="Print specification"
        description="Saved with the book and used for the cover and the KDP checks."
      >
        <form onSubmit={submit} className="grid gap-4 sm:grid-cols-[1fr_1fr_1fr_auto] sm:items-end">
          <SelectField
            label="Trim size"
            value={trimSizeId}
            onValueChange={setTrimSizeId}
            options={(trimSizes.data ?? []).map((t) => ({ value: t.id, label: t.label }))}
          />
          <TextField
            label="Page count"
            type="number"
            min={KDP_MIN_PAGES}
            value={pageCount}
            onChange={(e) => setPageCount(Number(e.target.value))}
          />
          <SelectField
            label="Ink and paper"
            value={paper}
            onValueChange={setPaper}
            options={toOptions(PAPER_LABELS)}
          />
          <Button type="submit" busy={save.isPending}>
            Save
          </Button>
        </form>
        <div className="mt-3">
          <ErrorMessage error={save.error} />
        </div>
      </Card>
      <CoverCalculator
        key={`${book.trimSizeId}-${book.pageCount}-${book.paper}`}
        initialTrimSizeId={book.trimSizeId}
        initialPageCount={book.pageCount}
        initialPaper={book.paper}
      />
    </div>
  );
};
