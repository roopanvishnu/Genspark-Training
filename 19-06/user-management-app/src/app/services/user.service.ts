
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { UserModel } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class UserService {
  private usersSubject = new BehaviorSubject<UserModel[]>([]);
  users$ = this.usersSubject.asObservable();

  constructor() {
    const stored = localStorage.getItem('users');
    if (stored) this.usersSubject.next(JSON.parse(stored));

    this.users$.subscribe(users => {
      localStorage.setItem('users', JSON.stringify(users));
    });

    console.log(' UserService instantiated');
  }

  addUser(user: UserModel): void {
    const current = this.usersSubject.value;
    const updated = [...current, user];
    console.log('User added:', user);
    console.log('Updated list:', updated);
    this.usersSubject.next(updated);
  }

  getUsers(): Observable<UserModel[]> {
    return this.users$;
  }
}