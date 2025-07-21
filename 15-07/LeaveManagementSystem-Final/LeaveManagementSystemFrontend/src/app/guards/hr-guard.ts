import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';

export const hrGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const role = localStorage.getItem('role');

  if (role === 'HR') {
    return true; 
  } else {
    router.navigate(['/dashboard']); 
    return false;
  }
};
