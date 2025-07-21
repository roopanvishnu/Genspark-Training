import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { User, UserRole, LoginRequest, RegisterRequest, AuthResponse } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor() {
    const storedUser = localStorage.getItem('currentUser');
    if (storedUser) {
      this.currentUserSubject.next(JSON.parse(storedUser));
    }
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    // Mock implementation - replace with actual HTTP call
    return new Observable(observer => {
      setTimeout(() => {
        if (request.email === 'manager@demo.com' && request.password === 'password') {
          const response: AuthResponse = {
            token: 'mock-jwt-token-manager',
            user: {
              id: '1',
              email: 'manager@demo.com',
              firstName: 'John',
              lastName: 'Manager',
              role: UserRole.Manager,
              isActive: true,
              createdAt: new Date(),
              lastLoginAt: new Date()
            },
            expiresAt: new Date(Date.now() + 24 * 60 * 60 * 1000)
          };
          this.setCurrentUser(response.user);
          localStorage.setItem('authToken', response.token);
          observer.next(response);
        } else if (request.email === 'member@demo.com' && request.password === 'password') {
          const response: AuthResponse = {
            token: 'mock-jwt-token-member',
            user: {
              id: '2',
              email: 'member@demo.com',
              firstName: 'Jane',
              lastName: 'Smith',
              role: UserRole.TeamMember,
              isActive: true,
              createdAt: new Date(),
              lastLoginAt: new Date()
            },
            expiresAt: new Date(Date.now() + 24 * 60 * 60 * 1000)
          };
          this.setCurrentUser(response.user);
          localStorage.setItem('authToken', response.token);
          observer.next(response);
        } else {
          observer.error({ message: 'Invalid credentials' });
        }
        observer.complete();
      }, 1000);
    });
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    // Mock implementation - replace with actual HTTP call
    return new Observable(observer => {
      setTimeout(() => {
        const response: AuthResponse = {
          token: 'mock-jwt-token-new-user',
          user: {
            id: Math.random().toString(),
            email: request.email,
            firstName: request.firstName,
            lastName: request.lastName,
            role: request.role,
            isActive: true,
            createdAt: new Date()
          },
          expiresAt: new Date(Date.now() + 24 * 60 * 60 * 1000)
        };
        this.setCurrentUser(response.user);
        localStorage.setItem('authToken', response.token);
        observer.next(response);
        observer.complete();
      }, 1000);
    });
  }

  logout(): void {
    localStorage.removeItem('currentUser');
    localStorage.removeItem('authToken');
    this.currentUserSubject.next(null);
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  isAuthenticated(): boolean {
    return this.getCurrentUser() !== null;
  }

  isManager(): boolean {
    const user = this.getCurrentUser();
    return user?.role === UserRole.Manager;
  }

  isTeamMember(): boolean {
    const user = this.getCurrentUser();
    return user?.role === UserRole.TeamMember;
  }

  private setCurrentUser(user: User): void {
    localStorage.setItem('currentUser', JSON.stringify(user));
    this.currentUserSubject.next(user);
  }
}