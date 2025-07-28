// import { Component, OnInit } from '@angular/core';
// import { ActivatedRoute } from '@angular/router';
// import { CommonModule } from '@angular/common';
// import { ToastrService } from 'ngx-toastr';

// import { LeaveRequestService } from '../../services/leave-request.service';
// import { LeaveAttachmentService } from '../../services/leave-attachment.service';
// import { LeaveBalanceService } from '../../services/leave-balance.service';

// import { LeaveRequestResponse } from '../../models/leave-request-response.model';
// import { LeaveBalanceForType } from '../../models/leave-balance.model';
// import { UpdateLeaveRequestStatusDto } from '../../models/update-leave-status.dto';
// import { LeaveAttachmentResponse } from '../../models/leave-attachment.model';

// @Component({
//   selector: 'app-leave-detail',
//   templateUrl: './leave-details.html',
//   styleUrls: ['./leave-details.css'],
//   standalone: true,
//   imports: [CommonModule]
// })
// export class LeaveDetails implements OnInit {
//   leaveId!: string;
//   leaveRequest?: LeaveRequestResponse;
//   attachments: any[] = [];
//   leaveBalanceForType?: LeaveBalanceForType;
//   errorMsg = '';
//   isLoading = false;

//   constructor(
//     private route: ActivatedRoute,
//     private leaveRequestService: LeaveRequestService,
//     private leaveAttachmentService: LeaveAttachmentService,
//     private leaveBalanceService: LeaveBalanceService,
//     private toastr: ToastrService
//   ) {}

//   ngOnInit(): void {
//     this.leaveId = this.route.snapshot.paramMap.get('id')!;
//     this.loadDetails();
//   }

//   loadDetails() {
//     this.isLoading = true;
//     this.leaveRequestService.getLeaveRequestById(this.leaveId).subscribe({
//       next: res => {
//         this.leaveRequest = res.data;
//         this.fetchAttachments();

//         if (this.leaveRequest) {
//           this.fetchBalanceForType(this.leaveRequest.userId, this.leaveRequest.leaveTypeId);
//         }

//         this.isLoading = false;
//       },
//       error: err => {
//         this.errorMsg = 'Failed to load leave details';
//         this.toastr.error(this.errorMsg, 'Error');
//         console.error(err);
//         this.isLoading = false;
//       }
//     });
//   }

//   fetchAttachments() {
//     this.leaveAttachmentService.getAttachmentsByLeaveRequestId(this.leaveId).subscribe({
//       next: res => {
//         this.attachments = res.data?.$values ?? [];
//       },
//       error: err => {
//         this.toastr.error('Failed to fetch attachments', 'Error');
//         console.error('Attachments fetch failed', err);
//       }
//     });
//   }


//   fetchBalanceForType(userId: string, leaveTypeId: string) {
//     this.leaveBalanceService.getLeaveBalanceForType(userId, leaveTypeId).subscribe({
//       next: res => {
//         this.leaveBalanceForType = res.data.leaveBalance;
//       },
//       error: err => {
//         this.toastr.error('Failed to fetch leave balance', 'Error');
//         console.error('Leave balance fetch failed', err);
//       }
//     });
//   }

//   updateStatus(newStatus: 'Approved' | 'Rejected') {
//     if (!this.leaveId) return;

//     const dto: UpdateLeaveRequestStatusDto = { status: newStatus };

//     this.leaveRequestService.updateLeaveStatus(this.leaveId, dto).subscribe({
//       next: () => {
//         this.toastr.success(`Leave request ${newStatus.toLowerCase()} successfully!`, 'Success');

//         // Reload updated data
//         this.leaveRequestService.getLeaveRequestById(this.leaveId).subscribe({
//           next: res => {
//             this.leaveRequest = res.data;
//           },
//           error: err => {
//             this.toastr.warning('Status updated, but failed to refresh data.', 'Partial Success');
//             console.error('Failed to reload leave request:', err);
//           }
//         });
//       },
//       error: err => {
//         const msg = err?.error?.message || 'Unknown error';
//         this.toastr.error(`Failed to update status. ${msg}`, 'Error');
//         console.error('Failed to update leave status:', err);
//       }
//     });
//   }
// }


import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

import { LeaveRequestService } from '../../services/leave-request.service';
import { LeaveAttachmentService } from '../../services/leave-attachment.service';
import { LeaveBalanceService } from '../../services/leave-balance.service';

import { LeaveRequestResponse } from '../../models/leave-request-response.model';
import { LeaveBalanceForType } from '../../models/leave-balance.model';
import { UpdateLeaveRequestStatusDto } from '../../models/update-leave-status.dto';
import { LeaveAttachmentResponse } from '../../models/leave-attachment.model';
import { AdminOverrideLeaveRequestDto } from '../../models/admin-override-leave-request.dto';
import { UserService } from '../../services/user.service';
import { User } from '../../models/user.model';

@Component({
  selector: 'app-leave-detail',
  templateUrl: './leave-details.html',
  styleUrls: ['./leave-details.css'],
  standalone: true,
  imports: [CommonModule, FormsModule]
})
export class LeaveDetails implements OnInit {
  leaveId!: string;
  leaveRequest?: LeaveRequestResponse;
  attachments: any[] = [];
  leaveBalanceForType?: LeaveBalanceForType;
  errorMsg = '';
  isLoading = false;
  showOverrideForm: boolean = false;


  // Admin override properties
  isAdmin = false;
  isHr = false;
  newStatus = '';
  overrideComment = '';
  currentUserRole = '';
  adminOverrideComment = '';


  constructor(
    private route: ActivatedRoute,
    private leaveRequestService: LeaveRequestService,
    private leaveAttachmentService: LeaveAttachmentService,
    private leaveBalanceService: LeaveBalanceService,
    private userService: UserService,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    this.leaveId = this.route.snapshot.paramMap.get('id')!;
    this.loadDetails();
    this.checkUserRole();
  }

  loadDetails() {
    this.isLoading = true;

    console.log('Fetching leave details for leaveId:', this.leaveId); // 👈 log the leaveId

    this.leaveRequestService.getLeaveRequestById(this.leaveId).subscribe({
      next: res => {
        this.leaveRequest = res.data;
        this.fetchAttachments();

        if (this.leaveRequest) {
          this.fetchBalanceForType(this.leaveRequest.userId, this.leaveRequest.leaveTypeId);
        }

        // Fetch the admin override comment
        this.leaveRequestService.getAdminOverrideComment(this.leaveId).subscribe({
          next: (res) => {
            this.adminOverrideComment = res.comment;
            console.log('Admin Override Comment:', this.adminOverrideComment);
          },
          error: (err) => {
            console.error('Failed to load admin override comment:', err);
          }
        });
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

  // Check user role from localStorage, sessionStorage, or JWT token
  checkUserRole() {
    // Method 1: Check localStorage/sessionStorage for user info
    const storedUser = localStorage.getItem('currentUser') ||
      localStorage.getItem('user') ||
      sessionStorage.getItem('currentUser') ||
      sessionStorage.getItem('user');

    if (storedUser) {
      try {
        const user = JSON.parse(storedUser);
        this.currentUserRole = user.role || user.userRole || '';
        this.isAdmin = this.currentUserRole === 'Admin';
        this.isHr = this.currentUserRole === 'HR';
        console.log('User role from storage:', this.currentUserRole, 'IsAdmin:', this.isAdmin, 'IsHR:', this.isHr);
        return;
      } catch (e) {
        console.error('Error parsing stored user:', e);
      }
    }

    // Method 2: Check JWT token
    const token = localStorage.getItem('accessToken')

    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        this.currentUserRole = payload.role || payload.userRole || payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || '';
        this.isAdmin = this.currentUserRole === 'Admin';
        this.isHr = this.currentUserRole === 'HR';
        console.log('User role from token:', this.currentUserRole, 'IsAdmin:', this.isAdmin, 'IsHR:', this.isHr);
        return;
      } catch (e) {
        console.error('Error parsing token:', e);
      }
    }

    // Method 3: Fallback - Get current user from API
    console.log('Fetching user role from API...');
    this.userService.getCurrentUser().subscribe({
      next: (user) => {
        this.currentUserRole = user.role || user.role || '';
        this.isAdmin = this.currentUserRole === 'Admin';
        this.isHr = this.currentUserRole === 'HR';
        console.log('User role from API:', this.currentUserRole, 'IsAdmin:', this.isAdmin, 'IsHR:', this.isHr);
      },
      error: (err) => {
        console.error('Failed to fetch user role from API:', err);
        console.warn('Could not determine user role from storage, token, or API');
      }
    });
  }

  // Check if HR can take action (approve/reject pending requests)
  canTakeHrAction(): boolean {
    return this.isHr && this.leaveRequest?.status === 'Pending';
  }

  // Check if Admin can override (override approved/rejected requests)
  canOverrideAsAdmin(): boolean {
    return this.isAdmin && this.leaveRequest?.status !== 'Pending';
  }

  // HR function to approve/reject pending requests
  updateStatus(newStatus: 'Approved' | 'Rejected') {
    if (!this.leaveId || !this.canTakeHrAction()) {
      this.toastr.warning('You are not authorized to perform this action', 'Warning');
      return;
    }

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

  // Admin function to override HR decisions
  adminOverride() {
    // Validate admin permissions
    if (!this.canOverrideAsAdmin()) {
      this.toastr.warning('You are not authorized to override this request', 'Warning');
      return;
    }

    // Validate required fields
    if (!this.newStatus) {
      this.toastr.warning('Please select a status', 'Warning');
      return;
    }

    if (!this.overrideComment.trim()) {
      this.toastr.warning('Please provide a comment for the override', 'Warning');
      return;
    }

    // Prepare the DTO
    const dto: AdminOverrideLeaveRequestDto = {
      newStatus: this.newStatus,
      comment: this.overrideComment.trim()
    };

    console.log('Admin override DTO:', dto);

    // Call the service method
    this.leaveRequestService.overrideLeaveRequestAsAdmin(this.leaveId, dto).subscribe({
      next: (response) => {
        console.log('Override response:', response);
        this.toastr.success('Leave request overridden by admin successfully!', 'Success');

        // Reset form
        this.newStatus = '';
        this.overrideComment = '';

        // Reload the leave request data to show updated status
        this.loadDetails();
      },
      error: err => {
        console.error('Override error:', err);
        const errorMsg = err?.error?.message || err?.message || 'Failed to override leave request';
        this.toastr.error(errorMsg, 'Error');
      }
    });
  }

  // Helper method to get status display class
  getStatusClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'pending':
        return 'status-pending';
      case 'approved':
        return 'status-approved';
      case 'rejected':
      case 'auto-rejected':
        return 'status-rejected';
      default:
        return '';
    }
  }
}