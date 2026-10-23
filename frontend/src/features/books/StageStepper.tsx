import { STAGE_LABELS, STAGES } from '@/constants/labels';
import type { BookStage } from '@/services/api/types';

interface StageStepperProps {
  current: BookStage;
  viewing: BookStage;
  onView: (stage: BookStage) => void;
}

/** The guided flow. Authors can look at any step up to the next one they have not reached yet. */
export const StageStepper = ({ current, viewing, onView }: StageStepperProps) => {
  const currentIndex = STAGES.indexOf(current);

  return (
    <ol className="flex flex-wrap gap-2" aria-label="Book progress">
      {STAGES.map((stage, index) => {
        const done = index < currentIndex;
        const reachable = index <= currentIndex + 1;
        const isViewing = stage === viewing;
        return (
          <li key={stage}>
            <button
              type="button"
              disabled={!reachable}
              aria-current={stage === current ? 'step' : undefined}
              onClick={() => onView(stage)}
              className={`flex items-center gap-2 rounded-full px-3 py-1.5 text-sm ring-1 transition disabled:cursor-not-allowed disabled:opacity-40 ${
                isViewing
                  ? 'bg-indigo-600 text-white ring-indigo-600'
                  : done
                    ? 'bg-emerald-50 text-emerald-800 ring-emerald-200'
                    : 'bg-white text-slate-700 ring-slate-300'
              }`}
            >
              <span className="grid size-5 place-items-center rounded-full bg-white/20 text-xs font-semibold">
                {done ? '✓' : index + 1}
              </span>
              {STAGE_LABELS[stage]}
            </button>
          </li>
        );
      })}
    </ol>
  );
};
