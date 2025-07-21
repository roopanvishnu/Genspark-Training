import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { ManagerService } from '../services/manager.service';
import { User, UserRole } from '../models/user.model';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl : './user-list.component.html'
})
export class UserListComponent implements OnInit {
  users: User[] = [];
  UserRole = UserRole;

  get activeUsers(): number {
    return this.users.filter(u => u.isActive).length;
  }

  get managers(): number {
    return this.users.filter(u => u.role === UserRole.Manager && u.isActive).length;
  }

  constructor(
    public authService: AuthService,
    private managerService: ManagerService
  ) {}

  ngOnInit(): void {
    if (this.authService.isManager()) {
      this.loadUsers();
    }
  }

  loadUsers(): void {
    this.managerService.getUsers().subscribe(users => {
      this.users = users;
    });
  }

  updateUserRole(userId: string, role: UserRole): void {
    this.managerService.updateUserRole(userId, role).subscribe(() => {
      this.loadUsers();
    });
  }

  softDeleteUser(userId: string): void {
    if (confirm('Are you sure you want to deactivate this user?')) {
      this.managerService.softDeleteUser(userId).subscribe(() => {
        this.loadUsers();
      });
    }
  }

  reactivateUser(userId: string): void {
    this.managerService.reactivateUser(userId).subscribe(() => {
      this.loadUsers();
    });
  }
}