import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient } from '@angular/common/http';
import { ProductService } from './services/product.service';
import { CategoryService } from './services/category.service';
import { UserService } from './services/user.service';
import { ColorService } from './services/color.service';
import { CartService } from './services/cart.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(),
    ProductService,
    CategoryService,
    UserService,
    ColorService,
    CartService
  ]
};
