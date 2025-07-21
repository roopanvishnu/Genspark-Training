
// import { Component, OnInit } from '@angular/core';
// import { CommonModule } from '@angular/common';
// import { HttpClient } from '@angular/common/http';
// import { AuthService } from '../../services/auth.service';
// import { Router, RouterModule } from '@angular/router';
// import { UserService } from '../../services/user.service';
// import { FormsModule } from '@angular/forms';

// @Component({
//   selector: 'app-home',
//   standalone: true,
//   imports: [CommonModule, RouterModule, FormsModule],
//   templateUrl: './home.html',
//   styleUrl: './home.css'
// })
// export class Home implements OnInit {
//   user: { fullName?: string; email?: string; role?: string } | null = null;
//   users: any[] = [];
//   currentPage = 1;
//   totalPages = 1;
//   pageSize = 10;
//   editingUser: any = null;




import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../services/auth.service';
import { Router, RouterModule } from '@angular/router';
import { UserService } from '../../services/user.service';
import { FormsModule } from '@angular/forms';
import { TaskService } from '../../services/task.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home implements OnInit {
  user: { fullName?: string; email?: string; role?: string } | null = null;
  users: any[] = [];
  currentPage = 1;
  totalPages = 1;
  pageSize = 10;
  editingUser: any = null;

  assignedTasks: any[] = [];
  recentTasks: any[] = [];
  urgentTasks: any[] = [];

  constructor(
    private http: HttpClient,
    private auth: AuthService,
    private router: Router,
    private userService: UserService,
    private taskService: TaskService
  ) { }

  ngOnInit() {
    const token = localStorage.getItem('accessToken');
    if (!token) return;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      this.user = {
        fullName: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
        email: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
        role: payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
      };

      if (!this.user.fullName && !this.user.email) {
        this.fetchUserFromApi();
      }

      if (this.user.role === 'Manager') {
        this.fetchUsers(this.currentPage);
      } else {
        this.fetchAssignedTasks();
      }
    } catch (err) {
      console.error('Invalid token format:', err);
      this.fetchUserFromApi();
    }
  }


  fetchUserFromApi() {
    this.http.get<any>('https://localhost:7120/api/v1/auth/me').subscribe({
      next: res => {
        this.user = res.data;
        if (this.user?.role === 'TeamMember') this.fetchAssignedTasks();
      },
      error: err => console.error('Failed to fetch user from /auth/me:', err)
    });
  }
  fetchAssignedTasks() {
    this.taskService.getAssignedTasks().subscribe({
      next: res => {
        const tasks = res.data || [];

        const filtered = tasks.filter(
          (t: { status: string; }) => ['Open', 'InProgress'].includes(t.status)
        );

        this.assignedTasks = filtered;

        this.urgentTasks = [...filtered]
          .filter(t => t.dueDate)
          .sort((a, b) => new Date(a.dueDate).getTime() - new Date(b.dueDate).getTime())
          .slice(0, 5);
      },
      error: err => console.error('❌ Failed to fetch assigned tasks:', err)
    });
  }


  getInitials(nameOrEmail: string | undefined): string {
    if (!nameOrEmail) return '?';
    const name = nameOrEmail.trim();
    const parts = name.split(' ');
    if (parts.length >= 2) return parts[0][0] + parts[1][0];
    return name[0];
  }

  logout() {
    console.clear();
    this.auth.logout();
    this.router.navigate(['/login']);
  }


  fetchUsers(page: number) {
    this.userService.getUsers(page, this.pageSize).subscribe({
      next: res => {
        this.users = res.data;
        this.totalPages = res.pagination.totalPages;
        this.currentPage = page;
      },
      error: err => {
        console.error('Failed to fetch users:', err);
      }
    });
  }

  getPages(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  goToPage(page: number) {
    if (page !== this.currentPage) {
      this.fetchUsers(page);
    }
  }

  startEditing(user: any) {
    this.editingUser = { ...user }; // clone to avoid mutating UI data
  }

  cancelEdit() {
    this.editingUser = null;
  }

  submitEdit() {
    const { id, fullName, role } = this.editingUser;
    if (!fullName || !role) {
      alert('All fields are required.');
      return;
    }

    this.userService.updateUser(id, { fullName, role }).subscribe({
      next: () => {
        alert('✅ User updated successfully.');
        this.editingUser = null;
        this.fetchUsers(this.currentPage);
      },
      error: err => {
        console.error('❌ Failed to update user:', err);
        alert('Failed to update user.');
      }
    });
  }

  deleteUser(userId: string) {
    if (!confirm('Are you sure you want to delete this user?')) return;

    this.userService.deleteUser(userId).subscribe({
      next: () => {
        console.log(`✅ User ${userId} deleted successfully.`);
        this.fetchUsers(this.currentPage);
      },
      error: err => {
        console.error(`❌ Failed to delete user ${userId}:`, err);
        alert('Failed to delete user.');
      }
    });
  }
}
