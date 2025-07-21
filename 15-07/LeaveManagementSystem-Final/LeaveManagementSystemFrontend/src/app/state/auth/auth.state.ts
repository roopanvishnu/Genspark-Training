export interface AuthState {
  user: any | null;         // use UserDto if available
  loading: boolean;
  error: string | null;
}

export const initialAuthState: AuthState = {
  user: null,
  loading: false,
  error: null
};
