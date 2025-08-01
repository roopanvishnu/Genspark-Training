import { Routes } from '@angular/router';
import { Products } from './products/products';
import { Login } from './login/login';
import { Cart } from './cart/cart';
import { Orders } from './orders/orders';
import { Categories } from './categories/categories';
import { Signup } from './signup/signup';
import { Colors } from './colors/colors';
import { News } from './news/news';
import { NewsManagement } from './news-management/news-management';
import { Contactus } from './contactus/contactus';
import { ContactManagement } from './contact-management/contact-management';

export const routes: Routes = [
    {path: '', component: Products},
    {path: 'colors', component: Colors},
    {path: 'categories', component: Categories},
    {path: 'orders', component : Orders},
    {path: 'cart', component: Cart},
    {path: 'login', component: Login},
    {path: 'signup', component: Signup},
    {path: 'news', component: News},
    {path: 'newsmanagement', component: NewsManagement},
    {path: 'contact-us', component: Contactus},
    {path: 'contactmanagement', component: ContactManagement},
];
