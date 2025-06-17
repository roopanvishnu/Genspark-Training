import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject } from '@angular/core';
import { UserLoginModel } from '../models/userloginmodel';

@Injectable({
  providedIn: 'root',
})
export class Userservice {
  private http = inject(HttpClient);

  constructor() {}

  validateUserLogin(user: UserLoginModel) {
    if (user.username.length < 3) {
      return null;
    } else {
      return this.loginApi(user).subscribe({
        next:(data:any)=>{
          localStorage.setItem('token', data.accessToken);
          console.log('Login successful, token stored:', data.accessToken);
        }
      });
    }
  }

  loginApi(user: UserLoginModel) {
    return this.http.post('https://dummyjson.com/auth/login', user);
  }

  getprofile() {
    const token = localStorage.getItem('token');
    if (!token) {
      return null;
    }
    const httpHeader = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.get('https://dummyjson.com/auth/me', {
      headers: httpHeader,
    });
  }

  logout() {
    localStorage.removeItem('token');
    console.log("logged out")
  } 
}
