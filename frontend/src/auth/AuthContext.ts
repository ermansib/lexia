import { createContext, useContext } from 'react';

export interface CurrentUser {
  name: string;
  signOut?: () => void;
}

export const CurrentUserContext = createContext<CurrentUser | null>(null);

export const useCurrentUser = (): CurrentUser => {
  const user = useContext(CurrentUserContext);
  if (!user) throw new Error('useCurrentUser must be used inside <LexiaAuthProvider>.');
  return user;
};
