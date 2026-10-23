export type AuthMode = 'dev' | 'oidc';

export const env = {
  apiBaseUrl: import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5000',
  authMode: (import.meta.env.VITE_AUTH_MODE ?? 'dev') as AuthMode,
  oidcAuthority: import.meta.env.VITE_OIDC_AUTHORITY ?? 'http://localhost:8080/realms/lexia',
  oidcClientId: import.meta.env.VITE_OIDC_CLIENT_ID ?? 'lexia-web',
};
