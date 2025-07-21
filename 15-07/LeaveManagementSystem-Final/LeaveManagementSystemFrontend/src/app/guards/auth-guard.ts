import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const token = localStorage.getItem('accessToken');

  if (token) {
    return true; // logged in, allow
  } else {
    router.navigate(['/login']); // not logged in, redirect to login
    return false;
  }
};
