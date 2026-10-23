import type { ReactNode } from 'react';
import { useParams } from 'react-router-dom';
import { EmptyState } from '@/components/ui/Feedback';
import { CoverCalculator } from './CoverCalculator';
import { JournalMaker } from './JournalMaker';
import { PrintChecker } from './PrintChecker';
import { PuzzleBookMaker } from './PuzzleBookMaker';

const TOOLS: Record<string, () => ReactNode> = {
  'puzzle-books': () => <PuzzleBookMaker />,
  journals: () => <JournalMaker />,
  cover: () => <CoverCalculator />,
  'print-check': () => <PrintChecker />,
};

export const ToolPage = () => {
  const { tool = '' } = useParams();
  const render = TOOLS[tool];
  return render ? render() : <EmptyState title="Tool not found" />;
};
