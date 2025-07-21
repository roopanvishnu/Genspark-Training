import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { CommonModule } from '@angular/common';

interface LeaveBalance {
  id: string;
  leaveTypeId: string;
  leaveTypeName: string;
  totalLeaves: number;
  usedLeaves: number;
  remainingLeaves: number;
}

interface UserLeaveBalance {
  userId: string;
  userName: string;
  leaveBalances: LeaveBalance[];
}

interface ApiResponse<T> {
  data: T;
}

@Component({
  selector: 'app-user-leave-balance',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './user-leave-balance.html'
})
export class UserLeaveBalanceComponent implements OnInit {
  userBalance: UserLeaveBalance | null = null;
  errorMsg = '';
  isLoading = false;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.fetchLeaveBalance();
  }

  fetchLeaveBalance() {
    this.isLoading = true;
    const token = localStorage.getItem('accessToken');
    const userId = localStorage.getItem('userId');

    if (!token || !userId) {
      this.errorMsg = 'Missing token or user ID.';
      this.isLoading = false;
      return;
    }

    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });

    this.http.get<ApiResponse<UserLeaveBalance>>(
      `http://localhost:5000/api/v1/leave-balance/user/${userId}`,
      { headers }
    ).subscribe({
      next: (res) => {
        this.userBalance = res?.data || null;
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMsg = err?.error?.message || 'Failed to load leave balance.';
        this.isLoading = false;
      }
    });
  }
}
