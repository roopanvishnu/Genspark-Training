import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private api = 'https://localhost:7120/api/v1/auth';

  constructor(private http: HttpClient, private router: Router) {}

  login(data: { email: string; password: string }): Observable<any> {
    return this.http.post<any>(`${this.api}/login`, data).pipe(
      tap(res => {
        localStorage.setItem('accessToken', res.accessToken);
        localStorage.setItem('refreshToken', res.refreshToken);
        console.log(res.accessToken)
        console.log(res.refreshToken)
      })
    );
  }

  register(data: { fullName: string; email: string; password: string; role: string }): Observable<any> {
    return this.http.post<any>(`${this.api}/register`, data);
  }

  logout() {
  localStorage.clear();
  this.router.navigate(['/login']);
}


  getAccessToken() {
    return localStorage.getItem('accessToken');
  }

  getRefreshToken() {
    return localStorage.getItem('refreshToken');
  }

  refreshToken(): Observable<any> {
    const token = this.getRefreshToken();
    return this.http.post<any>(`${this.api}/refresh`, token);
  }

  isLoggedIn(): boolean {
    return !!this.getAccessToken();
  }
}
