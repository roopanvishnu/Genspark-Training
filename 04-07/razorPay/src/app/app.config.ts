// src/app/app.config.ts
import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { Routes } from '@angular/router';
import { PaymentForm } from './components/payment-form/payment-form';
import { PaymentResult } from './components/payment-result/payment-result';

const routes: Routes = [
  { path: '', component: PaymentForm },
  { path: 'result', component: PaymentResult },
  { path: '**', redirectTo: '' } // fallback
];

export const appConfig: ApplicationConfig = {
  providers: [provideRouter(routes)],
};
