import { Card } from '@/components/ui/Card';
import { Badge } from '@/components/ui/Feedback';

const CHECKLIST = [
  'Search your main keyword on Amazon and note the top 10 books, their prices and review counts.',
  'Read the 1-, 2- and 3-star reviews of competing books and list what readers wish was different.',
  'Pick a niche where several books sell well (BSR under 100,000) but few have strong covers or many reviews.',
  'Write down 7 keyword phrases buyers would type. You will use them when you publish.',
];

export const ResearchPanel = () => (
  <Card
    title="Research the niche"
    description="Lexia's research tools (search volume, competition, opportunity score and review mining) are next on the roadmap."
    actions={<Badge tone="info">AI research coming soon</Badge>}
  >
    <p className="text-sm text-slate-700">
      Until then, this checklist covers what the research tools will automate:
    </p>
    <ol className="mt-3 list-decimal space-y-2 pl-5 text-sm text-slate-700">
      {CHECKLIST.map((step) => (
        <li key={step}>{step}</li>
      ))}
    </ol>
  </Card>
);
