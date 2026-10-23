import { useState } from 'react';
import { Button } from '@/components/ui/Button';
import { Card } from '@/components/ui/Card';
import { ErrorMessage } from '@/components/ui/Feedback';
import { SelectField, TextAreaField, TextField } from '@/components/ui/Field';
import { toOptions } from '@/components/ui/options';
import { DIFFICULTIES, PUZZLE_TYPE_LABELS } from '@/constants/labels';
import { useFileDownload } from '@/hooks/useFileDownload';
import { useTrimSizes } from '@/hooks/useTrimSizes';
import { studioApi } from '@/services/api/studioApi';
import type { Difficulty, PuzzleType, WordListInput } from '@/services/api/types';
import { parseWordLists, SAMPLE_WORD_LISTS } from './wordLists';

interface PuzzleBookMakerProps {
  initialTitle?: string;
  initialAuthor?: string | null;
  initialType?: PuzzleType;
  initialTrimSizeId?: string | null;
}

const DEFAULT_PUZZLE_COUNT = 50;
const MAX_PUZZLES = 200;

export const PuzzleBookMaker = ({
  initialTitle,
  initialAuthor,
  initialType,
  initialTrimSizeId,
}: PuzzleBookMakerProps) => {
  const trimSizes = useTrimSizes();
  const [title, setTitle] = useState(initialTitle ?? 'Large Print Puzzles');
  const [author, setAuthor] = useState(initialAuthor ?? '');
  const [puzzleType, setPuzzleType] = useState<PuzzleType>(initialType ?? 'Sudoku');
  const [difficulty, setDifficulty] = useState<Difficulty>('Easy');
  const [trimSizeId, setTrimSizeId] = useState(initialTrimSizeId ?? '8.5x11');
  const [puzzleCount, setPuzzleCount] = useState(DEFAULT_PUZZLE_COUNT);
  const [includeSolutions, setIncludeSolutions] = useState(true);
  const [wordListText, setWordListText] = useState(SAMPLE_WORD_LISTS);
  const download = useFileDownload(studioApi.puzzleBook);

  const wordLists: WordListInput[] = parseWordLists(wordListText);

  const generate = () =>
    download.mutate({
      title,
      author: author || null,
      trimSizeId,
      puzzleType,
      difficulty,
      puzzleCount,
      wordLists: puzzleType === 'WordSearch' ? wordLists : null,
      includeSolutions,
      seed: null,
    });

  return (
    <Card
      title="Puzzle book maker"
      description="Generate a print-ready KDP interior: title page, instructions, puzzles and an answer key, padded to KDP's rules."
    >
      <div className="grid gap-4 sm:grid-cols-2">
        <TextField label="Book title" value={title} onChange={(e) => setTitle(e.target.value)} />
        <TextField label="Author (optional)" value={author} onChange={(e) => setAuthor(e.target.value)} />
        <SelectField
          label="Puzzle type"
          value={puzzleType}
          onValueChange={setPuzzleType}
          options={toOptions(PUZZLE_TYPE_LABELS)}
        />
        <SelectField
          label="Difficulty"
          value={difficulty}
          onValueChange={setDifficulty}
          options={DIFFICULTIES.map((d) => ({ value: d, label: d }))}
        />
        <SelectField
          label="Trim size"
          value={trimSizeId}
          onValueChange={setTrimSizeId}
          options={(trimSizes.data ?? []).map((t) => ({ value: t.id, label: t.label }))}
        />
        <TextField
          label="Number of puzzles"
          type="number"
          min={1}
          max={MAX_PUZZLES}
          value={puzzleCount}
          onChange={(e) => setPuzzleCount(Number(e.target.value))}
        />
      </div>

      {puzzleType === 'WordSearch' && (
        <div className="mt-4">
          <TextAreaField
            label="Word lists"
            hint="One list per line: a theme, a colon, then comma-separated words. Lists repeat if you ask for more puzzles than lists."
            rows={5}
            value={wordListText}
            onChange={(e) => setWordListText(e.target.value)}
          />
        </div>
      )}

      <label className="mt-4 flex items-center gap-2 text-sm text-slate-700">
        <input
          type="checkbox"
          checked={includeSolutions}
          onChange={(e) => setIncludeSolutions(e.target.checked)}
        />
        Include an answer key at the back
      </label>

      <div className="mt-6 flex flex-wrap items-center gap-3">
        <Button onClick={generate} busy={download.isPending}>
          Generate interior PDF
        </Button>
        {download.isSuccess && (
          <span className="text-sm text-emerald-700">Downloaded. Upload it to KDP as your manuscript.</span>
        )}
      </div>
      <div className="mt-3">
        <ErrorMessage error={download.error} />
      </div>
    </Card>
  );
};
