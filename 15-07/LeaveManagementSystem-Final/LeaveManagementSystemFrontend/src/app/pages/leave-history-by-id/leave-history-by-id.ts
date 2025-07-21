import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { LeaveRequestService } from '../../services/leave-request.service';
import { LeaveBalanceService } from '../../services/leave-balance.service';
import { LeaveAttachmentService } from '../../services/leave-attachment.service';
import { LeaveRequestResponse } from '../../models/leave-request-response.model';
import { LeaveBalanceForType } from '../../models/leave-balance.model';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-leave-detail',
  templateUrl: './leave-history-by-id.html',
  styleUrls: ['./leave-history-by-id.css'],
  standalone: true,
  imports: [CommonModule]
})
export class LeaveHistoryById implements OnInit {
  leaveId!: string;
  leaveRequest!: LeaveRequestResponse;
  attachments: any[] = [];
  leaveBalanceForType!: LeaveBalanceForType;
  errorMsg = '';
  isLoading = false;
  selectedFile: File | null = null;

  constructor(
    private route: ActivatedRoute,
    private leaveRequestService: LeaveRequestService,
    private leaveBalanceService: LeaveBalanceService,
    private leaveAttachmentService: LeaveAttachmentService,
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
        this.fetchLeaveBalanceForType();
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
        this.toastr.error('Failed to load attachments', 'Error');
        console.error(err);
      }
    });
  }


  fetchLeaveBalanceForType() {
    const { userId, leaveTypeId } = this.leaveRequest;
    this.leaveBalanceService.getLeaveBalanceForType(userId, leaveTypeId).subscribe({
      next: res => {
        this.leaveBalanceForType = res.data.leaveBalance;
      },
      error: err => {
        this.toastr.error('Failed to load leave balance', 'Error');
        console.error(err);
      }
    });
  }

  updateStatus(newStatus: 'Approved' | 'Rejected') {
    if (!this.leaveId) return;

    this.leaveRequestService.updateLeaveStatus(this.leaveId, { status: newStatus }).subscribe({
      next: () => {
        this.toastr.success(`Leave ${newStatus.toLowerCase()} successfully`, 'Status Updated');
        this.loadDetails();
      },
      error: err => {
        const msg = err?.error?.message || 'Unknown error';
        this.toastr.error(`Failed to update status: ${msg}`, 'Error');
        console.error(err);
      }
    });
  }

  cancelLeaveRequest() {
    if (!this.leaveId) return;

    const confirmed = confirm('Are you sure you want to cancel this leave request?');
    if (!confirmed) return;

    this.leaveRequestService.cancelLeaveRequest(this.leaveId).subscribe({
      next: () => {
        this.toastr.success('Leave request cancelled successfully', 'Cancelled');
        this.loadDetails();
      },
      error: err => {
        const msg = err?.error?.message || 'Cancellation failed';
        this.toastr.error(msg, 'Error');
        console.error(err);
      }
    });
  }

  deleteAttachment(attachmentId: string) {
    const confirmed = confirm('Are you sure you want to delete this attachment?');
    if (!confirmed) return;

    this.leaveAttachmentService.deleteAttachment(attachmentId).subscribe({
      next: () => {
        this.attachments = this.attachments.filter(att => att.id !== attachmentId);
        this.toastr.success('Attachment deleted successfully');
      },
      error: err => {
        this.toastr.error('Failed to delete attachment', 'Error');
        console.error(err);
      }
    });
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
    }
  }

  uploadAttachment() {
    if (!this.selectedFile || !this.leaveId) return;

    this.leaveAttachmentService.uploadAttachment(this.leaveId, this.selectedFile).subscribe({
      next: () => {
        this.toastr.success('Attachment uploaded successfully');
        this.selectedFile = null;
        this.fetchAttachments(); // refresh list
      },
      error: err => {
        this.toastr.error('Failed to upload file', 'Error');
        console.error(err);
      }
    });
  }

}
