// app.routes.ts
import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { ProductDetails } from './pages/product-details/product-details';
import { Login } from './auth/login/login';
import { AuthGuard } from './auth/auth.guard';
import { Register } from './auth/register/register';
import { About } from './pages/about/about';

export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'register', component: Register }, // ✅ NEW
  { path: 'about', component: About },
  {
    path: 'products',
    canActivate: [AuthGuard],
    children: [
      { path: '', component: Home },
      { path: ':id', component: ProductDetails }
    ]
  },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: '**', redirectTo: 'login' }
];
