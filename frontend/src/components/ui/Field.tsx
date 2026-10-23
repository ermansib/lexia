import type { Option } from './options';
import {
  useId,
  type InputHTMLAttributes,
  type ReactNode,
  type SelectHTMLAttributes,
  type TextareaHTMLAttributes,
} from 'react';

const CONTROL =
  'mt-1 block w-full rounded-lg border-0 bg-white px-3 py-2 text-sm text-slate-900 ring-1 ring-slate-300 placeholder:text-slate-400 focus:ring-2 focus:ring-indigo-600';

interface FieldShellProps {
  label: string;
  hint?: string;
  children: (id: string) => ReactNode;
}

const FieldShell = ({ label, hint, children }: FieldShellProps) => {
  const id = useId();
  return (
    <div>
      <label htmlFor={id} className="block text-sm font-medium text-slate-700">
        {label}
      </label>
      {children(id)}
      {hint && <p className="mt-1 text-xs text-slate-500">{hint}</p>}
    </div>
  );
};

type TextFieldProps = InputHTMLAttributes<HTMLInputElement> & { label: string; hint?: string };

export const TextField = ({ label, hint, ...rest }: TextFieldProps) => (
  <FieldShell label={label} hint={hint}>
    {(id) => <input id={id} className={CONTROL} {...rest} />}
  </FieldShell>
);

type TextAreaFieldProps = TextareaHTMLAttributes<HTMLTextAreaElement> & { label: string; hint?: string };

export const TextAreaField = ({ label, hint, ...rest }: TextAreaFieldProps) => (
  <FieldShell label={label} hint={hint}>
    {(id) => <textarea id={id} className={CONTROL} {...rest} />}
  </FieldShell>
);

type SelectFieldProps<T extends string> = Omit<SelectHTMLAttributes<HTMLSelectElement>, 'onChange'> & {
  label: string;
  hint?: string;
  options: Option<T>[];
  onValueChange: (value: T) => void;
};

export const SelectField = <T extends string>({
  label,
  hint,
  options,
  onValueChange,
  ...rest
}: SelectFieldProps<T>) => (
  <FieldShell label={label} hint={hint}>
    {(id) => (
      <select id={id} className={CONTROL} onChange={(e) => onValueChange(e.target.value as T)} {...rest}>
        {options.map((o) => (
          <option key={o.value} value={o.value}>
            {o.label}
          </option>
        ))}
      </select>
    )}
  </FieldShell>
);
