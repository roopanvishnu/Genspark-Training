import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { LeaveTypeService } from '../../services/leave-type.service';
import { LeaveRequestService } from '../../services/leave-request.service';
import { UserService } from '../../services/user.service';
import { LeaveCalendarComponent } from '../../components/leave-calendar/leave-calendar';
import { LeaveRequestResponse } from '../../models/leave-request-response.model';

@Component({
  selector: 'app-hr-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, LeaveCalendarComponent],
  templateUrl: './hr-dashboard.html',
  styleUrls: ['./hr-dashboard.css'],
})
export class HrDashboardComponent implements OnInit {
  leavesToday: LeaveRequestResponse[] = [];

  private leaveRequestService = inject(LeaveRequestService);
  private userService = inject(UserService);
  private leaveTypeService = inject(LeaveTypeService);

  stats = {
    totalLeaves: 0,
    approved: 0,
    pending: 0,
    rejected: 0,
    autoRejected: 0,
    totalUsers: 0,
    leaveTypes: 0,
  };

  recentLeaves: any[] = [];
  loading = false;

  ngOnInit(): void {
    this.fetchData();
  }

  fetchData(): void {
    this.loading = true;
    const today = new Date();
    const startOfMonth = new Date(today.getFullYear(), today.getMonth(), 1);

    this.leaveRequestService.getAllLeaveRequests(1, 1000, '', '', '', '').subscribe((res:any) => {
      const allLeaves = res?.data?.data?.$values || [];

      const thisMonthLeaves = allLeaves
        .filter((x: any) => new Date(x.startDate) >= startOfMonth)
        .sort((a: any, b: any) => new Date(a.startDate).getTime() - new Date(b.startDate).getTime());

      this.stats.totalLeaves = thisMonthLeaves.length;
      this.stats.approved = thisMonthLeaves.filter((x: any) => x.status === 'Approved').length;
      this.stats.pending = thisMonthLeaves.filter((x: any) => x.status === 'Pending').length;
      this.stats.rejected = thisMonthLeaves.filter((x: any) => x.status === 'Rejected').length;
      this.stats.autoRejected = thisMonthLeaves.filter((x: any) => x.status === 'Auto-Rejected').length;

      this.recentLeaves = thisMonthLeaves.filter((x: any) => new Date(x.startDate) >= today).slice(0, 5);
      this.loading = false;

      this.leavesToday = thisMonthLeaves.filter((x: LeaveRequestResponse) => {
        const start = new Date(x.startDate);
        const end = new Date(x.endDate);
        return start <= today && today <= end;
      });

    });

    this.userService.getUsers(1, 1000).subscribe((res) => {
      const users = res?.data?.$values || [];
      this.stats.totalUsers = users.length;
    });

    this.leaveTypeService.getLeaveTypes().subscribe((types) => {
      this.stats.leaveTypes = types.length;
    });
  }
}
