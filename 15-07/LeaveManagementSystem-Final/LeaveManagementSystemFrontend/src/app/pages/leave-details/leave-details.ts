import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';

import { LeaveRequestService } from '../../services/leave-request.service';
import { LeaveAttachmentService } from '../../services/leave-attachment.service';
import { LeaveBalanceService } from '../../services/leave-balance.service';

import { LeaveRequestResponse } from '../../models/leave-request-response.model';
import { LeaveBalanceForType } from '../../models/leave-balance.model';
import { UpdateLeaveRequestStatusDto } from '../../models/update-leave-status.dto';
import { LeaveAttachmentResponse } from '../../models/leave-attachment.model';

@Component({
  selector: 'app-leave-detail',
  templateUrl: './leave-details.html',
  styleUrls: ['./leave-details.css'],
  standalone: true,
  imports: [CommonModule]
})
export class LeaveDetails implements OnInit {
  leaveId!: string;
  leaveRequest?: LeaveRequestResponse;
  attachments: any[] = [];
  leaveBalanceForType?: LeaveBalanceForType;
  errorMsg = '';
  isLoading = false;

  constructor(
    private route: ActivatedRoute,
    private leaveRequestService: LeaveRequestService,
    private leaveAttachmentService: LeaveAttachmentService,
    private leaveBalanceService: LeaveBalanceService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.leaveId = this.route.snapshot.paramMap.get('id')!;
    this.loadDetails();
  }

  loadDetails() {
    this.isLoading = true;
    this.leaveRequestService.getLeaveRequestById(this.leaveId).subscribe({
      next: res => {
        this.leaveRequest = res.data;
        this.fetchAttachments();

        if (this.leaveRequest) {
          this.fetchBalanceForType(this.leaveRequest.userId, this.leaveRequest.leaveTypeId);
        }

        this.isLoading = false;
      },
      error: err => {
        this.errorMsg = 'Failed to load leave details';
        this.toastr.error(this.errorMsg, 'Error');
        console.error(err);
        this.isLoading = false;
      }
    });
  }

  fetchAttachments() {
    this.leaveAttachmentService.getAttachmentsByLeaveRequestId(this.leaveId).subscribe({
      next: res => {
        this.attachments = res.data?.$values ?? [];
      },
      error: err => {
        this.toastr.error('Failed to fetch attachments', 'Error');
        console.error('Attachments fetch failed', err);
      }
    });
  }


  fetchBalanceForType(userId: string, leaveTypeId: string) {
    this.leaveBalanceService.getLeaveBalanceForType(userId, leaveTypeId).subscribe({
      next: res => {
        this.leaveBalanceForType = res.data.leaveBalance;
      },
      error: err => {
        this.toastr.error('Failed to fetch leave balance', 'Error');
        console.error('Leave balance fetch failed', err);
      }
    });
  }

  updateStatus(newStatus: 'Approved' | 'Rejected') {
    if (!this.leaveId) return;

    const dto: UpdateLeaveRequestStatusDto = { status: newStatus };

    this.leaveRequestService.updateLeaveStatus(this.leaveId, dto).subscribe({
      next: () => {
        this.toastr.success(`Leave request ${newStatus.toLowerCase()} successfully!`, 'Success');

        // Reload updated data
        this.leaveRequestService.getLeaveRequestById(this.leaveId).subscribe({
          next: res => {
            this.leaveRequest = res.data;
          },
          error: err => {
            this.toastr.warning('Status updated, but failed to refresh data.', 'Partial Success');
            console.error('Failed to reload leave request:', err);
          }
        });
      },
      error: err => {
        const msg = err?.error?.message || 'Unknown error';
        this.toastr.error(`Failed to update status. ${msg}`, 'Error');
        console.error('Failed to update leave status:', err);
      }
    });
  }
}
