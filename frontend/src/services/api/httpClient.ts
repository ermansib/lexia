import { env } from '@/config/env';
import type { ProblemDetails } from './types';

/** An API failure carrying the server's problem details, so screens can show a useful message. */
export class ApiError extends Error {
  readonly status: number;
  readonly problem: ProblemDetails;

  constructor(status: number, problem: ProblemDetails) {
    super(ApiError.describe(problem, status));
    this.status = status;
    this.problem = problem;
  }

  static describe(problem: ProblemDetails, status: number): string {
    const fieldErrors = problem.errors ? Object.values(problem.errors).flat() : [];
    if (fieldErrors.length > 0) return fieldErrors.join(' ');
    return problem.detail ?? problem.title ?? `Request failed (${status}).`;
  }
}

type TokenProvider = () => string | undefined;
let getToken: TokenProvider = () => undefined;

/** The auth layer registers how to read the current access token. */
export const setTokenProvider = (provider: TokenProvider) => {
  getToken = provider;
};

const send = async (path: string, init: RequestInit = {}): Promise<Response> => {
  const headers = new Headers(init.headers);
  const token = getToken();
  if (token) headers.set('Authorization', `Bearer ${token}`);
  if (init.body && !headers.has('Content-Type')) headers.set('Content-Type', 'application/json');

  const response = await fetch(`${env.apiBaseUrl}${path}`, { ...init, headers });
  if (!response.ok) {
    const problem = (await response.json().catch(() => ({}))) as ProblemDetails;
    throw new ApiError(response.status, problem);
  }
  return response;
};

export const http = {
  get: async <T>(path: string): Promise<T> => (await send(path)).json() as Promise<T>,
  post: async <T>(path: string, body: unknown): Promise<T> =>
    (await send(path, { method: 'POST', body: JSON.stringify(body) })).json() as Promise<T>,
  put: async <T>(path: string, body: unknown): Promise<T> =>
    (await send(path, { method: 'PUT', body: JSON.stringify(body) })).json() as Promise<T>,
  delete: async (path: string): Promise<void> => {
    await send(path, { method: 'DELETE' });
  },
  /** Posts JSON and returns the response as a downloadable file. */
  download: async (path: string, body: unknown): Promise<{ blob: Blob; fileName: string }> => {
    const response = await send(path, { method: 'POST', body: JSON.stringify(body) });
    const disposition = response.headers.get('Content-Disposition') ?? '';
    const match = /filename\*?=(?:UTF-8'')?"?([^";]+)"?/i.exec(disposition);
    return { blob: await response.blob(), fileName: match ? decodeURIComponent(match[1]) : 'lexia.pdf' };
  },
};
