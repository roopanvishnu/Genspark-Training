
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private tokenKey = 'jwt_token';
  private apiUrl = 'https://api.freeprojectapi.com/api/BusBooking';

  constructor(private http: HttpClient, private router: Router) {}

  login(username: string, password: string) {
  return this.http
    .post<any>(`${this.apiUrl}/login`, {
      userName: username,
      password: password
    })
    .pipe(
      tap((res) => {
        console.log('API Response:', res); 

        if (res && res.token) {
          localStorage.setItem(this.tokenKey, res.token);
          this.router.navigate(['/home']);
        } else {
          console.warn('No token received');
        }
      })
    );
}


  logout() {
    localStorage.removeItem(this.tokenKey);
    this.router.navigate(['/home']);
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem(this.tokenKey);
  }

  getAuthHeaders() {
    const token = localStorage.getItem(this.tokenKey);
    return { Authorization: `Bearer ${token}` };
  }
}
