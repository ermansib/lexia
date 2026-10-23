import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { Card } from '@/components/ui/Card';
import { ErrorMessage, Badge } from '@/components/ui/Feedback';
import { SelectField, TextField } from '@/components/ui/Field';
import { toOptions } from '@/components/ui/options';
import { KDP_MIN_PAGES, PAPER_LABELS } from '@/constants/labels';
import { useTrimSizes } from '@/hooks/useTrimSizes';
import { studioApi } from '@/services/api/studioApi';
import type { CoverDimensionsDto, PaperType } from '@/services/api/types';
import { CoverDiagram } from './CoverDiagram';

interface CoverCalculatorProps {
  initialTrimSizeId?: string | null;
  initialPageCount?: number | null;
  initialPaper?: PaperType | null;
}

const DEFAULT_TRIM = '6x9';
const DEFAULT_PAGES = 120;

export const CoverCalculator = ({
  initialTrimSizeId,
  initialPageCount,
  initialPaper,
}: CoverCalculatorProps) => {
  const trimSizes = useTrimSizes();
  const [trimSizeId, setTrimSizeId] = useState(initialTrimSizeId ?? DEFAULT_TRIM);
  const [pageCount, setPageCount] = useState(initialPageCount ?? DEFAULT_PAGES);
  const [paper, setPaper] = useState<PaperType>(initialPaper ?? 'BlackInkWhitePaper');

  const cover = useQuery({
    queryKey: ['cover', trimSizeId, pageCount, paper],
    queryFn: () => studioApi.cover(trimSizeId, pageCount, paper),
    enabled: pageCount > 0,
    retry: false,
  });

  return (
    <Card
      title="Cover size calculator"
      description="Exact full-wrap cover size for KDP paperbacks, with spine and bleed."
    >
      <div className="grid gap-4 sm:grid-cols-3">
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
      </div>
      <div className="mt-6">
        <ErrorMessage error={cover.error} />
        {cover.data && <CoverResult cover={cover.data} />}
      </div>
    </Card>
  );
};

const CoverResult = ({ cover }: { cover: CoverDimensionsDto }) => (
  <div className="grid gap-6 lg:grid-cols-[1fr_1.4fr]">
    <dl className="grid grid-cols-2 content-start gap-3 text-sm">
      <Stat label="Full cover" value={`${cover.fullWidthIn}" × ${cover.fullHeightIn}"`} />
      <Stat label="Spine width" value={`${cover.spineWidthIn}"`} />
      <Stat
        label={`Pixels at ${cover.requiredDpi} DPI`}
        value={`${cover.fullWidthPx} × ${cover.fullHeightPx}`}
      />
      <Stat label="Bleed on each edge" value={`${cover.bleedIn}"`} />
      <div className="col-span-2">
        {cover.spineTextAllowed ? (
          <Badge tone="success">Spine text allowed</Badge>
        ) : (
          <Badge tone="warning">No spine text under 80 pages</Badge>
        )}
      </div>
    </dl>
    <CoverDiagram cover={cover} />
  </div>
);

const Stat = ({ label, value }: { label: string; value: string }) => (
  <div className="rounded-lg bg-slate-50 p-3">
    <dt className="text-xs text-slate-500">{label}</dt>
    <dd className="mt-1 font-semibold text-slate-900">{value}</dd>
  </div>
);
