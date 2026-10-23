import { NavLink, Outlet } from 'react-router-dom';
import { useCurrentUser } from '@/auth/AuthContext';
import { Badge } from '@/components/ui/Feedback';

interface NavItem {
  to: string;
  label: string;
}

const MAIN_NAV: NavItem[] = [{ to: '/', label: 'My books' }];

const TOOLS_NAV: NavItem[] = [
  { to: '/tools/puzzle-books', label: 'Puzzle book maker' },
  { to: '/tools/journals', label: 'Journal maker' },
  { to: '/tools/cover', label: 'Cover calculator' },
  { to: '/tools/print-check', label: 'KDP print checker' },
];

/** Features on the roadmap, shown so authors know what is coming. */
const COMING_SOON = ['Niche research', 'AI co-writer', 'AI illustrations', 'Sales tracking'];

const linkClass = ({ isActive }: { isActive: boolean }) =>
  `block rounded-lg px-3 py-2 text-sm ${isActive ? 'bg-indigo-50 font-medium text-indigo-700' : 'text-slate-700 hover:bg-slate-100'}`;

const NavSection = ({ title, items }: { title?: string; items: NavItem[] }) => (
  <div>
    {title && (
      <p className="mb-1 hidden px-3 text-xs font-semibold tracking-wide text-slate-400 uppercase md:block">
        {title}
      </p>
    )}
    <ul className="flex gap-1 md:block md:space-y-1">
      {items.map((item) => (
        <li key={item.to}>
          <NavLink to={item.to} end className={(state) => `whitespace-nowrap ${linkClass(state)}`}>
            {item.label}
          </NavLink>
        </li>
      ))}
    </ul>
  </div>
);

export const AppShell = () => {
  const user = useCurrentUser();

  return (
    <div className="min-h-screen bg-slate-50 md:grid md:grid-cols-[240px_1fr]">
      <aside className="border-b border-slate-200 bg-white p-4 md:min-h-screen md:border-r md:border-b-0">
        <div className="mb-3 flex items-center gap-2 px-3 md:mb-6">
          <img src="/lexia.svg" alt="" className="size-7" />
          <span className="text-lg font-bold text-slate-900">Lexia</span>
        </div>
        <nav className="flex gap-1 overflow-x-auto md:block md:space-y-6" aria-label="Main">
          <NavSection items={MAIN_NAV} />
          <NavSection title="Free tools" items={TOOLS_NAV} />
          <div className="hidden md:block">
            <p className="mb-1 px-3 text-xs font-semibold tracking-wide text-slate-400 uppercase">
              Coming next
            </p>
            <ul className="space-y-1">
              {COMING_SOON.map((label) => (
                <li
                  key={label}
                  className="flex items-center justify-between px-3 py-2 text-sm text-slate-400"
                >
                  {label} <Badge>Soon</Badge>
                </li>
              ))}
            </ul>
          </div>
        </nav>
        <div className="mt-8 hidden border-t border-slate-100 px-3 pt-4 text-sm text-slate-600 md:block">
          <p className="font-medium text-slate-800">{user.name}</p>
          {user.signOut && (
            <button type="button" onClick={user.signOut} className="mt-1 text-indigo-600 hover:underline">
              Sign out
            </button>
          )}
        </div>
      </aside>
      <main className="mx-auto w-full max-w-5xl p-4 sm:p-8">
        <Outlet />
      </main>
    </div>
  );
};
