import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { lazy, Suspense } from 'react';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import { LexiaAuthProvider } from '@/auth/LexiaAuthProvider';
import { AppShell } from '@/components/layout/AppShell';
import { EmptyState } from '@/components/ui/Feedback';
import { DashboardPage } from '@/features/dashboard/DashboardPage';

const BookPage = lazy(() => import('@/features/books/BookPage').then((m) => ({ default: m.BookPage })));
const ToolPage = lazy(() => import('@/features/tools/ToolPage').then((m) => ({ default: m.ToolPage })));

const queryClient = new QueryClient({
  defaultOptions: { queries: { refetchOnWindowFocus: false, retry: 1 } },
});

export const App = () => (
  <QueryClientProvider client={queryClient}>
    <LexiaAuthProvider>
      <BrowserRouter>
        <Suspense fallback={<p className="p-8 text-slate-500">Loading…</p>}>
          <Routes>
            <Route element={<AppShell />}>
              <Route index element={<DashboardPage />} />
              <Route path="books/:bookId" element={<BookPage />} />
              <Route path="tools/:tool" element={<ToolPage />} />
              <Route path="*" element={<EmptyState title="Page not found" />} />
            </Route>
          </Routes>
        </Suspense>
      </BrowserRouter>
    </LexiaAuthProvider>
  </QueryClientProvider>
);
