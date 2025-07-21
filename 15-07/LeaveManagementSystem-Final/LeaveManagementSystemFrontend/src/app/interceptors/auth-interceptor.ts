import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { AuthStateService } from '../services/auth-state.service';
import { catchError, switchMap, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const authState = inject(AuthStateService);

  const accessToken = localStorage.getItem('accessToken');
  const refreshToken = localStorage.getItem('refreshToken');

  // Skip refresh loop
  if (req.url.includes('/auth/refresh')) {
    return next(req);
  }

  let clonedRequest = req;

  if (accessToken) {
    clonedRequest = req.clone({
      setHeaders: { Authorization: `Bearer ${accessToken}` }
    });
  }

  return next(clonedRequest).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && refreshToken) {
        return authService.refreshToken(refreshToken).pipe(
          switchMap((newToken: { accessToken: string }) => {
            localStorage.setItem('accessToken', newToken.accessToken);
            const retriedRequest = req.clone({
              setHeaders: { Authorization: `Bearer ${newToken.accessToken}` }
            });
            return next(retriedRequest);
          }),
          catchError(err => {
            authService.logout(); // Logout if refresh fails
            return throwError(() => err);
          })
        );
      }

      return throwError(() => error);
    })
  );
};
