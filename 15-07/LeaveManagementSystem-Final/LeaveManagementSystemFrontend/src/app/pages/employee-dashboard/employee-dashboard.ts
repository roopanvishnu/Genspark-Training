import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LeaveRequestService } from '../../services/leave-request.service';
import { LeaveCalendarComponent } from '../../components/leave-calendar/leave-calendar';
import { LeaveRequestResponse } from '../../models/leave-request-response.model';
import { AuthStateService } from '../../services/auth-state.service';

@Component({
  selector: 'app-employee-dashboard',
  standalone: true,
  imports: [CommonModule, LeaveCalendarComponent],
  templateUrl: './employee-dashboard.html',
  styleUrls: ['./employee-dashboard.css']
})
export class EmployeeDashboardComponent implements OnInit {
  private leaveRequestService = inject(LeaveRequestService);
  private authState = inject(AuthStateService);

  stats = {
    totalLeaves: 0,
    approved: 0,
    pending: 0,
    rejected: 0,
    autoRejected: 0
  };

  recentLeaves: LeaveRequestResponse[] = [];
  userId: string | null = null;

  ngOnInit(): void {
    this.authState.userId$?.subscribe((uid: string | null) => {
      this.userId = uid;
      if (this.userId) {
        this.fetchLeaveData();
      }
    });
  }

  fetchLeaveData(): void {
    this.leaveRequestService
      .getAllLeaveRequests(1, 1000, '', '', '', 'desc')
      .subscribe((res:any) => {
        const allLeaves = (res.data?.data?.$values ?? []) as LeaveRequestResponse[];

        const myLeaves = allLeaves.filter((leave) => leave.userId === this.userId);

        this.stats.totalLeaves = myLeaves.length;
        this.stats.approved = myLeaves.filter((x) => x.status === 'Approved').length;
        this.stats.pending = myLeaves.filter((x) => x.status === 'Pending').length;
        this.stats.rejected = myLeaves.filter((x) => x.status === 'Rejected').length;
        this.stats.autoRejected = myLeaves.filter((x) => x.status === 'Auto-Rejected').length;

        this.recentLeaves = myLeaves
          .sort((a, b) => new Date(b.createdAt!).getTime() - new Date(a.createdAt!).getTime())
          .slice(0, 5);
      });
  }
}
