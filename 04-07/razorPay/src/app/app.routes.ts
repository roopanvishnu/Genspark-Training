
import { Routes } from '@angular/router';
import { PaymentForm } from './components/payment-form/payment-form';
import { PaymentResult } from './components/payment-result/payment-result';

export const appRoutes: Routes = [
  { path: '', component: PaymentForm },
  { path: 'result', component: PaymentResult },
  { path: '**', redirectTo: '' }
];
