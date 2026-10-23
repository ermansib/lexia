import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { booksApi, type BookListParams } from '@/services/api/booksApi';
import type { BookDto } from '@/services/api/types';

const BOOKS_KEY = ['books'] as const;

export const useBookList = (params: BookListParams) =>
  useQuery({ queryKey: [...BOOKS_KEY, 'list', params], queryFn: () => booksApi.list(params) });

export const useBook = (id: string) =>
  useQuery({ queryKey: [...BOOKS_KEY, id], queryFn: () => booksApi.get(id) });

/** Wraps a book update so the cached book and lists refresh after it succeeds. */
export const useBookMutation = <T>(mutationFn: (input: T) => Promise<BookDto>) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn,
    onSuccess: (book) => {
      queryClient.setQueryData([...BOOKS_KEY, book.id], book);
      void queryClient.invalidateQueries({ queryKey: [...BOOKS_KEY, 'list'] });
    },
  });
};

export const useDeleteBook = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: booksApi.remove,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: BOOKS_KEY }),
  });
};
