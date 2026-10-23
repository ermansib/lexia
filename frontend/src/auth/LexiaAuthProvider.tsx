import { WebStorageStateStore } from 'oidc-client-ts';
import { useEffect, type ReactNode } from 'react';
import { AuthProvider, useAuth } from 'react-oidc-context';
import { env } from '@/config/env';
import { setTokenProvider } from '@/services/api/httpClient';
import { CurrentUserContext } from './AuthContext';

const DEV_USER = { name: 'Local Author' };

const oidcConfig = {
  authority: env.oidcAuthority,
  client_id: env.oidcClientId,
  redirect_uri: window.location.origin,
  post_logout_redirect_uri: window.location.origin,
  scope: 'openid profile email',
  userStore: new WebStorageStateStore({ store: window.sessionStorage }),
  // Remove the ?code= query from the URL once sign-in completes.
  onSigninCallback: () => window.history.replaceState({}, document.title, window.location.pathname),
};

const OidcGate = ({ children }: { children: ReactNode }) => {
  const auth = useAuth();

  useEffect(() => {
    setTokenProvider(() => auth.user?.access_token);
  }, [auth.user]);

  if (auth.isLoading) return <p className="p-8 text-slate-500">Signing you in…</p>;

  if (!auth.isAuthenticated) {
    return (
      <main className="grid min-h-screen place-items-center bg-slate-50 p-6">
        <div className="max-w-sm rounded-2xl bg-white p-8 text-center shadow-sm ring-1 ring-slate-200">
          <h1 className="text-2xl font-semibold text-slate-900">Welcome to Lexia</h1>
          <p className="mt-2 text-slate-600">Research, create and publish books on Amazon KDP.</p>
          {auth.error && <p className="mt-4 text-sm text-red-600">{auth.error.message}</p>}
          <button
            type="button"
            onClick={() => void auth.signinRedirect()}
            className="mt-6 w-full rounded-lg bg-indigo-600 px-4 py-2 font-medium text-white hover:bg-indigo-700"
          >
            Sign in
          </button>
        </div>
      </main>
    );
  }

  const user = {
    name: auth.user?.profile.name ?? auth.user?.profile.preferred_username ?? 'Author',
    signOut: () => void auth.signoutRedirect(),
  };
  return <CurrentUserContext.Provider value={user}>{children}</CurrentUserContext.Provider>;
};

/** Keycloak sign-in in "oidc" mode; a fixed local author in "dev" mode. */
export const LexiaAuthProvider = ({ children }: { children: ReactNode }) => {
  if (env.authMode === 'dev') {
    return <CurrentUserContext.Provider value={DEV_USER}>{children}</CurrentUserContext.Provider>;
  }

  return (
    <AuthProvider {...oidcConfig}>
      <OidcGate>{children}</OidcGate>
    </AuthProvider>
  );
};
