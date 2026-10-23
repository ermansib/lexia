import type { ReactNode } from 'react';

export const ErrorMessage = ({ error }: { error: unknown }) =>
  error ? (
    <p role="alert" className="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-700 ring-1 ring-red-200">
      {error instanceof Error ? error.message : 'Something went wrong.'}
    </p>
  ) : null;

type Tone = 'neutral' | 'success' | 'warning' | 'danger' | 'info';

const TONES: Record<Tone, string> = {
  neutral: 'bg-slate-100 text-slate-700',
  success: 'bg-emerald-50 text-emerald-700 ring-emerald-200',
  warning: 'bg-amber-50 text-amber-800 ring-amber-200',
  danger: 'bg-red-50 text-red-700 ring-red-200',
  info: 'bg-indigo-50 text-indigo-700 ring-indigo-200',
};

export const Badge = ({ tone = 'neutral', children }: { tone?: Tone; children: ReactNode }) => (
  <span
    className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ring-1 ring-transparent ${TONES[tone]}`}
  >
    {children}
  </span>
);

export const EmptyState = ({ title, children }: { title: string; children?: ReactNode }) => (
  <div className="rounded-2xl border-2 border-dashed border-slate-200 p-10 text-center">
    <p className="font-medium text-slate-800">{title}</p>
    {children && <div className="mt-2 text-sm text-slate-600">{children}</div>}
  </div>
);
