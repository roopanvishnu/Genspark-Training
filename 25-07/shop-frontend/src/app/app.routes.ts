import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { Home } from './pages/home/home';
import { UserComponent } from './pages/user/user';
import { Contactus } from './pages/contactus/contactus';

export const routes: Routes = [
  { path: '', component: Login },
  { path: 'home', component: Home},
  { path : 'users', component : UserComponent},
  { path :'contactus', component : Contactus}
];
