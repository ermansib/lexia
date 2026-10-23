import { useMutation } from '@tanstack/react-query';
import { useState } from 'react';
import { Button } from '@/components/ui/Button';
import { Card } from '@/components/ui/Card';
import { Badge, ErrorMessage } from '@/components/ui/Feedback';
import { SelectField, TextField } from '@/components/ui/Field';
import { toOptions } from '@/components/ui/options';
import { PAPER_LABELS } from '@/constants/labels';
import { useTrimSizes } from '@/hooks/useTrimSizes';
import { studioApi } from '@/services/api/studioApi';
import type { CheckPrintReadinessRequest, PrintReadinessReportDto } from '@/services/api/types';

const INITIAL: CheckPrintReadinessRequest = {
  trimSizeId: '6x9',
  paper: 'BlackInkWhitePaper',
  pageCount: 200,
  hasBleed: false,
  pageWidthIn: 6,
  pageHeightIn: 9,
  insideMarginIn: 0.5,
  outsideMarginIn: 0.5,
  topMarginIn: 0.5,
  bottomMarginIn: 0.5,
  lowestImageDpi: 300,
  allFontsEmbedded: true,
  hasSpineText: true,
};

type NumberKey = {
  [K in keyof CheckPrintReadinessRequest]: CheckPrintReadinessRequest[K] extends number | null ? K : never;
}[keyof CheckPrintReadinessRequest];

const NUMBER_FIELDS: { key: NumberKey; label: string; step: number }[] = [
  { key: 'pageCount', label: 'Pages', step: 1 },
  { key: 'pageWidthIn', label: 'PDF page width (in)', step: 0.001 },
  { key: 'pageHeightIn', label: 'PDF page height (in)', step: 0.001 },
  { key: 'insideMarginIn', label: 'Inside (gutter) margin (in)', step: 0.125 },
  { key: 'outsideMarginIn', label: 'Outside margin (in)', step: 0.125 },
  { key: 'topMarginIn', label: 'Top margin (in)', step: 0.125 },
  { key: 'bottomMarginIn', label: 'Bottom margin (in)', step: 0.125 },
  { key: 'lowestImageDpi', label: 'Lowest image DPI', step: 1 },
];

const FLAGS: { key: 'hasBleed' | 'allFontsEmbedded' | 'hasSpineText'; label: string }[] = [
  { key: 'hasBleed', label: 'Pages have bleed' },
  { key: 'allFontsEmbedded', label: 'All fonts embedded' },
  { key: 'hasSpineText', label: 'Cover has spine text' },
];

export const PrintChecker = () => {
  const trimSizes = useTrimSizes();
  const [form, setForm] = useState(INITIAL);
  const check = useMutation({ mutationFn: studioApi.printCheck });
  const update = <K extends keyof CheckPrintReadinessRequest>(key: K, value: CheckPrintReadinessRequest[K]) =>
    setForm((f) => ({ ...f, [key]: value }));

  return (
    <Card
      title="KDP print checker"
      description="Catch the issues that most often get a paperback rejected, before you upload."
    >
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <SelectField
          label="Trim size"
          value={form.trimSizeId}
          onValueChange={(v) => update('trimSizeId', v)}
          options={(trimSizes.data ?? []).map((t) => ({ value: t.id, label: t.label }))}
        />
        <SelectField
          label="Ink and paper"
          value={form.paper}
          onValueChange={(v) => update('paper', v)}
          options={toOptions(PAPER_LABELS)}
        />
        {NUMBER_FIELDS.map(({ key, label, step }) => (
          <TextField
            key={key}
            label={label}
            type="number"
            step={step}
            value={form[key] ?? ''}
            onChange={(e) => update(key, e.target.value === '' ? null : Number(e.target.value))}
          />
        ))}
      </div>
      <div className="mt-4 flex flex-wrap gap-6">
        {FLAGS.map(({ key, label }) => (
          <label key={key} className="flex items-center gap-2 text-sm text-slate-700">
            <input type="checkbox" checked={form[key]} onChange={(e) => update(key, e.target.checked)} />
            {label}
          </label>
        ))}
      </div>
      <div className="mt-6">
        <Button onClick={() => check.mutate(form)} busy={check.isPending}>
          Check my book
        </Button>
      </div>
      <div className="mt-4 space-y-3">
        <ErrorMessage error={check.error} />
        {check.data && <PrintReport report={check.data} />}
      </div>
    </Card>
  );
};

export const PrintReport = ({ report }: { report: PrintReadinessReportDto }) => (
  <div className="space-y-3">
    {report.isReady ? (
      <Badge tone="success">
        Ready for KDP{report.warningCount > 0 ? `, with ${report.warningCount} warning(s)` : ''}
      </Badge>
    ) : (
      <Badge tone="danger">{report.errorCount} issue(s) KDP will reject</Badge>
    )}
    <ul className="space-y-2">
      {report.issues.map((issue) => (
        <li key={issue.code} className="rounded-lg p-3 text-sm ring-1 ring-slate-200">
          <div className="flex items-center gap-2">
            <Badge tone={issue.severity === 'Error' ? 'danger' : 'warning'}>{issue.severity}</Badge>
            <span className="text-slate-900">{issue.message}</span>
          </div>
          <p className="mt-1 text-slate-600">Fix: {issue.fix}</p>
        </li>
      ))}
    </ul>
  </div>
);
