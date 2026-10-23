import { http } from './httpClient';
import type {
  CheckPrintReadinessRequest,
  CoverDimensionsDto,
  GenerateJournalRequest,
  GeneratePuzzleBookRequest,
  PaperType,
  PrintReadinessReportDto,
  TrimSizeDto,
} from './types';

const BASE = '/api/v1/studio';

export const studioApi = {
  trimSizes: () => http.get<TrimSizeDto[]>(`${BASE}/kdp/trim-sizes`),
  cover: (trimSizeId: string, pageCount: number, paper: PaperType) =>
    http.get<CoverDimensionsDto>(
      `${BASE}/kdp/cover?${new URLSearchParams({ trimSizeId, pageCount: String(pageCount), paper })}`,
    ),
  printCheck: (body: CheckPrintReadinessRequest) =>
    http.post<PrintReadinessReportDto>(`${BASE}/kdp/print-check`, body),
  puzzleBook: (body: GeneratePuzzleBookRequest) => http.download(`${BASE}/interiors/puzzle-books`, body),
  journal: (body: GenerateJournalRequest) => http.download(`${BASE}/interiors/journals`, body),
};
