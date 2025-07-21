import { Routes } from '@angular/router';
import { UserForm } from './pages/user-form/user-form';
import { UserList } from './pages/user-list/user-list';

export const routes: Routes = [
  { path: '', redirectTo: 'form', pathMatch: 'full' },
  { path: 'form', component: UserForm },
  { path: 'users', component: UserList },
];
