import { useState } from 'react';
import { Button } from '@/components/ui/Button';
import { Card } from '@/components/ui/Card';
import { ErrorMessage } from '@/components/ui/Feedback';
import { SelectField, TextField } from '@/components/ui/Field';
import { toOptions } from '@/components/ui/options';
import { JOURNAL_STYLE_LABELS, KDP_MIN_PAGES } from '@/constants/labels';
import { useFileDownload } from '@/hooks/useFileDownload';
import { useTrimSizes } from '@/hooks/useTrimSizes';
import { studioApi } from '@/services/api/studioApi';
import type { JournalStyle } from '@/services/api/types';

interface JournalMakerProps {
  initialTitle?: string;
  initialAuthor?: string | null;
  initialTrimSizeId?: string | null;
}

const DEFAULT_PAGES = 120;

export const JournalMaker = ({ initialTitle, initialAuthor, initialTrimSizeId }: JournalMakerProps) => {
  const trimSizes = useTrimSizes();
  const [title, setTitle] = useState(initialTitle ?? 'My Journal');
  const [author, setAuthor] = useState(initialAuthor ?? '');
  const [style, setStyle] = useState<JournalStyle>('Lined');
  const [trimSizeId, setTrimSizeId] = useState(initialTrimSizeId ?? '6x9');
  const [pageCount, setPageCount] = useState(DEFAULT_PAGES);
  const download = useFileDownload(studioApi.journal);

  return (
    <Card title="Journal maker" description="Lined, dot grid, graph or blank journals with KDP-safe margins.">
      <div className="grid gap-4 sm:grid-cols-2">
        <TextField label="Book title" value={title} onChange={(e) => setTitle(e.target.value)} />
        <TextField label="Author (optional)" value={author} onChange={(e) => setAuthor(e.target.value)} />
        <SelectField
          label="Page style"
          value={style}
          onValueChange={setStyle}
          options={toOptions(JOURNAL_STYLE_LABELS)}
        />
        <SelectField
          label="Trim size"
          value={trimSizeId}
          onValueChange={setTrimSizeId}
          options={(trimSizes.data ?? []).map((t) => ({ value: t.id, label: t.label }))}
        />
        <TextField
          label="Pages"
          type="number"
          min={KDP_MIN_PAGES}
          value={pageCount}
          onChange={(e) => setPageCount(Number(e.target.value))}
        />
      </div>
      <div className="mt-6">
        <Button
          busy={download.isPending}
          onClick={() => download.mutate({ title, author: author || null, trimSizeId, style, pageCount })}
        >
          Generate interior PDF
        </Button>
      </div>
      <div className="mt-3">
        <ErrorMessage error={download.error} />
      </div>
    </Card>
  );
};
