import { Injectable } from '@angular/core';
import { User } from './user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private dummyUsers: User[] = [
    { username: 'admin', password: 'admin123' },
    { username: 'user', password: 'user123' }
  ];

  login(user: User): boolean {
    const match = this.dummyUsers.find(
      u => u.username === user.username && u.password === user.password
    );
    if (match) {
      sessionStorage.setItem('loggedInUser', JSON.stringify(match));
      return true;
    }
    return false;
  }

  getLoggedInUser(): User | null {
    const stored = sessionStorage.getItem('loggedInUser');
    return stored ? JSON.parse(stored) : null;
  }

  logout(): void {
    sessionStorage.removeItem('loggedInUser');
  }
}
