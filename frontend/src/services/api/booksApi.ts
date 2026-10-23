import { http } from './httpClient';
import type { BookDto, BookStage, BookType, PagedResult, PaperType } from './types';

const BASE = '/api/v1/books';

export interface BookListParams {
  stage?: BookStage;
  type?: BookType;
  search?: string;
  page?: number;
  pageSize?: number;
}

export const booksApi = {
  list: (params: BookListParams = {}) => {
    const query = new URLSearchParams(
      Object.entries(params)
        .filter(([, value]) => value !== undefined && value !== '')
        .map(([key, value]) => [key, String(value)]),
    );
    return http.get<PagedResult<BookDto>>(`${BASE}?${query}`);
  },
  get: (id: string) => http.get<BookDto>(`${BASE}/${id}`),
  create: (body: { title: string; type: BookType; authorName: string | null }) =>
    http.post<BookDto>(BASE, body),
  updateDetails: (
    id: string,
    body: {
      title: string;
      subtitle: string | null;
      authorName: string | null;
      seriesName: string | null;
      seriesNumber: number | null;
    },
  ) => http.put<BookDto>(`${BASE}/${id}/details`, body),
  setPrintSpec: (id: string, body: { trimSizeId: string; pageCount: number; paper: PaperType }) =>
    http.put<BookDto>(`${BASE}/${id}/print-spec`, body),
  setListing: (id: string, body: { description: string | null; keywords: string[] }) =>
    http.put<BookDto>(`${BASE}/${id}/listing`, body),
  moveToStage: (id: string, stage: BookStage) => http.put<BookDto>(`${BASE}/${id}/stage`, { stage }),
  remove: (id: string) => http.delete(`${BASE}/${id}`),
};
