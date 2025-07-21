
import { Component, OnDestroy, OnInit } from '@angular/core';
import { BehaviorSubject, combineLatest, debounceTime, distinctUntilChanged, map, Observable, startWith, Subject, takeUntil } from 'rxjs';
import { UserModel } from '../../models/user.model';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { UserService } from '../../services/user.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './user-list.html',
  styleUrls: ['./user-list.scss']
})
export class UserList implements OnInit, OnDestroy {
  users$!: Observable<UserModel[]>;
  filteredUsers$ = new BehaviorSubject<UserModel[]>([]);

  searchControl = new FormControl('');
  roleControl = new FormControl('');

  private destroy$ = new Subject<void>();

  constructor(private userService: UserService) {}

  ngOnInit(): void {
    this.users$ = this.userService.getUsers();

    this.users$.subscribe(users => {
      console.log('[UserList] Users from service:', users);
    });

    combineLatest([
      this.users$,
      this.searchControl.valueChanges.pipe(startWith(''), debounceTime(400), distinctUntilChanged()),
      this.roleControl.valueChanges.pipe(startWith(''))
    ])
      .pipe(
        map(([users, search, role]) => {
          const term = (search || '').toLowerCase();
          return users.filter(user => {
            const matchesSearch = user.username.toLowerCase().includes(term) || user.role.toLowerCase().includes(term);
            const matchesRole = !role || user.role === role;
            return matchesSearch && matchesRole;
          });
        }),
        takeUntil(this.destroy$)
      )
      .subscribe(filtered => this.filteredUsers$.next(filtered));
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}