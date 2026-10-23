export interface Option<T extends string> {
  value: T;
  label: string;
}

/** Turns a label map such as { Easy: 'Easy puzzles' } into select options. */
export const toOptions = <T extends string>(labels: Record<T, string>): Option<T>[] =>
  (Object.keys(labels) as T[]).map((value) => ({ value, label: labels[value] }));
