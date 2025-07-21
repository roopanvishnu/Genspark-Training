import { CanActivateFn, Router, ActivatedRouteSnapshot } from '@angular/router';
import { inject } from '@angular/core';

export const RoleGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const router = inject(Router);
  const token = localStorage.getItem('accessToken');

  if (!token) {
    router.navigate(['/login']);
    return false;
  }

  const payload = JSON.parse(atob(token.split('.')[1]));
  const userRole = payload['https://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

  const expectedRole = route.data['role'] as string;

  if (userRole === expectedRole) return true;

  router.navigate(['/home']);
  return false;
};
