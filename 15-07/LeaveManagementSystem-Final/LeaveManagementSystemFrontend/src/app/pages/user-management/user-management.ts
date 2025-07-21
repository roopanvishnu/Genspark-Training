import { CommonModule } from '@angular/common';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, NavigationEnd } from '@angular/router';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';

import { UserService } from '../../services/user.service';
import { UserDto } from '../../models/user-dto.model';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-management.html',
  styleUrls: ['./user-management.css']
})
export class UserManagement implements OnInit, OnDestroy {
  allUsers: UserDto[] = [];
  filteredUsers: UserDto[] = [];
  paginatedUsers: UserDto[] = [];

  page = 1;
  pageSize = 10;
  totalPages = 1;

  searchTerm = '';
  roleFilter = '';
  sortBy: keyof UserDto = 'createdAt';
  sortOrder: 'asc' | 'desc' = 'desc';

  private routerSub!: Subscription;

  constructor(
    private userService: UserService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.loadUsers();

    this.routerSub = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => this.loadUsers());
  }

  ngOnDestroy() {
    if (this.routerSub) this.routerSub.unsubscribe();
  }

  loadUsers() {
    this.userService.getUsers(1, 1000).subscribe({
      next: (res) => {
        this.allUsers = res.data?.$values || [];
        this.resetToFirstPageAndFilter();
      },
      error: (err) => console.error('Error loading users:', err),
    });
  }

  resetToFirstPageAndFilter() {
    this.page = 1;
    this.applyFilters();
  }

  applyFilters() {
    let result = [...this.allUsers];

    const term = this.searchTerm.trim().toLowerCase();
    if (term) {
      result = result.filter(u =>
        (u.username || '').toLowerCase().includes(term) ||
        (u.email || '').toLowerCase().includes(term)
      );
    }

    if (this.roleFilter) {
      result = result.filter(u => u.role === this.roleFilter);
    }

result.sort((a, b) => {
  const aVal = a[this.sortBy];
  const bVal = b[this.sortBy];

  if (aVal == null || bVal == null) return 0;

  // Handle date sorting (only for `createdAt`)
  if (this.sortBy === 'createdAt') {
    return this.sortOrder === 'asc'
      ? new Date(aVal as string).getTime() - new Date(bVal as string).getTime()
      : new Date(bVal as string).getTime() - new Date(aVal as string).getTime();
  }

  // Handle string sorting
  if (typeof aVal === 'string' && typeof bVal === 'string') {
    return this.sortOrder === 'asc'
      ? aVal.localeCompare(bVal)
      : bVal.localeCompare(aVal);
  }

  // Handle boolean sorting
  if (typeof aVal === 'boolean' && typeof bVal === 'boolean') {
    return this.sortOrder === 'asc'
      ? Number(aVal) - Number(bVal)
      : Number(bVal) - Number(aVal);
  }

  // Handle number sorting (fallback)
  if (typeof aVal === 'number' && typeof bVal === 'number') {
    return this.sortOrder === 'asc' ? aVal - bVal : bVal - aVal;
  }

  return 0;
});


    this.filteredUsers = result;
    this.totalPages = Math.max(1, Math.ceil(this.filteredUsers.length / this.pageSize));
    this.page = Math.min(this.page, this.totalPages);

    this.paginateUsers();
  }

  paginateUsers() {
    const start = (this.page - 1) * this.pageSize;
    const end = start + this.pageSize;
    this.paginatedUsers = this.filteredUsers.slice(start, end);
  }

  changePage(offset: number) {
    const newPage = this.page + offset;
    if (newPage >= 1 && newPage <= this.totalPages) {
      this.page = newPage;
      this.paginateUsers();
    }
  }

  deleteUser(id: string) {
    if (confirm('Are you sure you want to delete this user?')) {
      this.userService.deleteUser(id).subscribe({
        next: () => {
          this.toastr.success('User deleted successfully', 'Deleted');
          this.loadUsers();
        },
        error: (err) => {
          console.error('Delete Failed:', err);
          const msg = err?.error?.message || 'Failed to delete user';
          this.toastr.error(msg, 'Error');
        },
      });
    }
  }

  navigateToAddUser() {
    this.router.navigate(['/users/add']);
  }

  navigateToEditUser(id: string) {
    this.router.navigate(['/users/edit', id]);
  }

  navigateToUserDetail(userId: string) {
    this.router.navigate(['/users', userId, 'detail']);
  }
}
