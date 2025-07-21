import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { User, UserRole } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class ManagerService {
  private usersSubject = new BehaviorSubject<User[]>([]);
  public users$ = this.usersSubject.asObservable();

  constructor() {
    this.loadMockUsers();
  }

  getUsers(): Observable<User[]> {
    return new Observable(observer => {
      setTimeout(() => {
        observer.next(this.usersSubject.value);
        observer.complete();
      }, 300);
    });
  }

  updateUserRole(userId: string, role: UserRole): Observable<User> {
    return new Observable(observer => {
      setTimeout(() => {
        const users = this.usersSubject.value;
        const userIndex = users.findIndex(u => u.id === userId);
        
        if (userIndex === -1) {
          observer.error({ message: 'User not found' });
          return;
        }

        users[userIndex] = { ...users[userIndex], role };
        this.usersSubject.next([...users]);
        observer.next(users[userIndex]);
        observer.complete();
      }, 500);
    });
  }

  softDeleteUser(userId: string): Observable<void> {
    return new Observable(observer => {
      setTimeout(() => {
        const users = this.usersSubject.value;
        const userIndex = users.findIndex(u => u.id === userId);
        
        if (userIndex === -1) {
          observer.error({ message: 'User not found' });
          return;
        }

        users[userIndex] = { ...users[userIndex], isActive: false };
        this.usersSubject.next([...users]);
        observer.next();
        observer.complete();
      }, 500);
    });
  }

  reactivateUser(userId: string): Observable<void> {
    return new Observable(observer => {
      setTimeout(() => {
        const users = this.usersSubject.value;
        const userIndex = users.findIndex(u => u.id === userId);
        
        if (userIndex === -1) {
          observer.error({ message: 'User not found' });
          return;
        }

        users[userIndex] = { ...users[userIndex], isActive: true };
        this.usersSubject.next([...users]);
        observer.next();
        observer.complete();
      }, 500);
    });
  }

  private loadMockUsers(): void {
    const mockUsers: User[] = [
      {
        id: '1',
        email: 'manager@demo.com',
        firstName: 'Mugundhan',
        lastName: 'Manager',
        role: UserRole.Manager,
        isActive: true,
        createdAt: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000),
        lastLoginAt: new Date()
      },
      {
        id: '2',
        email: 'member@demo.com',
        firstName: 'Reka',
        lastName: 'T',
        role: UserRole.TeamMember,
        isActive: true,
        createdAt: new Date(Date.now() - 20 * 24 * 60 * 60 * 1000),
        lastLoginAt: new Date(Date.now() - 2 * 60 * 60 * 1000)
      },
      {
        id: '3',
        email: 'tarun@demo.com',
        firstName: 'Tarun',
        lastName: 'Vijaay',
        role: UserRole.TeamMember,
        isActive: true,
        createdAt: new Date(Date.now() - 15 * 24 * 60 * 60 * 1000),
        lastLoginAt: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000)
      },
      {
        id: '4',
        email: 'roopan@demo.com',
        firstName: 'Roopan',
        lastName: 'vishnu',
        role: UserRole.TeamMember,
        isActive: false,
        createdAt: new Date(Date.now() - 45 * 24 * 60 * 60 * 1000),
        lastLoginAt: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000)
      }
    ];

    this.usersSubject.next(mockUsers);
  }
}