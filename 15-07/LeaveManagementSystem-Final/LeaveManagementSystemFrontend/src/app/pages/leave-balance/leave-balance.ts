import { Component, OnInit } from '@angular/core';
import { LeaveBalanceService } from '../../services/leave-balance.service';
import { UserService } from '../../services/user.service';
import { LeaveTypeService } from '../../services/leave-type.service';
import { AuthStateService } from '../../services/auth-state.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-leave-balance',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './leave-balance.html',
  styleUrls: ['./leave-balance.css']
})
export class LeaveBalance implements OnInit {
  balances: any[] = [];
  errorMsg = '';
  users: any[] = [];
  leaveTypes: any[] = [];

  currentUserId = localStorage.getItem('userId');
  currentRole = localStorage.getItem('role') || '';

  selectedUserIdForView = '';
  selectedUserIdForInit = '';
  selectedUserIdForReset = '';
  selectedUserIdForDeduct = '';
  selectedUserIdForTypeInit = '';
  selectedLeaveTypeIdForTypeInit = '';
  standardLeaveCountForTypeInit = 0;

  deductLeaveTypeId = '';
  deductDays = 1;

  selectedUserBalance: any = null;

  constructor(
    private leaveBalanceService: LeaveBalanceService,
    private userService: UserService,
    private leaveTypeService: LeaveTypeService,
    private auth: AuthStateService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.loadSelfBalance();
    this.loadLeaveTypes();
    if (this.isAdminOrHR()) this.loadAllUsers();
  }

  isAdminOrHR(): boolean {
    return this.currentRole === 'Admin' || this.currentRole === 'HR';
  }

  loadSelfBalance() {
    if (!this.currentUserId) return;

    this.leaveBalanceService.getLeaveBalance(this.currentUserId).subscribe({
      next: (res: any) => {
        this.balances = res?.data?.leaveBalances?.$values || [];
      },
      error: () => {
        this.errorMsg = 'Failed to load leave balance.';
        this.toastr.error('Failed to load leave balance.', 'Error');
      }
    });
  }

  loadAllUsers() {
    this.userService.getUsers(1, 100).subscribe({
      next: (res) => {
        this.users = res.data?.$values || [];
      },
      error: () => this.toastr.error('Failed to fetch users.', 'Error')
    });
  }

  loadLeaveTypes() {
    this.leaveTypeService.getLeaveTypes().subscribe({
      next: (types) => {
        this.leaveTypes = types;
      },
      error: () => this.toastr.error('Failed to fetch leave types.', 'Error')
    });
  }

  viewOtherUserBalance() {
    if (!this.selectedUserIdForView) return;

    this.leaveBalanceService.getLeaveBalance(this.selectedUserIdForView).subscribe({
      next: (res: any) => {
        this.selectedUserBalance = res.data?.leaveBalances?.$values || [];
      },
      error: () => this.toastr.error('Failed to fetch user balance.', 'Error')
    });
  }

  initializeBalance() {
    if (!this.selectedUserIdForInit) return;

    this.leaveBalanceService.initializeLeaveBalance(this.selectedUserIdForInit).subscribe({
      next: () => this.toastr.success('Leave balances initialized.', 'Success'),
      error: () => this.toastr.error('Failed to initialize leave balances.', 'Error')
    });
  }

  initializeTypeBalance() {
    if (
      !this.selectedUserIdForTypeInit ||
      !this.selectedLeaveTypeIdForTypeInit ||
      this.standardLeaveCountForTypeInit <= 0
    )
      return;

    this.leaveBalanceService
      .initializeLeaveType(
        this.selectedUserIdForTypeInit,
        this.selectedLeaveTypeIdForTypeInit,
        this.standardLeaveCountForTypeInit
      )
      .subscribe({
        next: () => this.toastr.success('Leave type initialized.', 'Success'),
        error: () => this.toastr.error('Failed to initialize leave type.', 'Error')
      });
  }

  resetBalance() {
    if (!this.selectedUserIdForReset) return;

    this.leaveBalanceService.resetLeaveBalance(this.selectedUserIdForReset).subscribe({
      next: () => this.toastr.success('Leave balances reset.', 'Success'),
      error: () => this.toastr.error('Failed to reset balances.', 'Error')
    });
  }

  deductLeave() {
    if (!this.selectedUserIdForDeduct || !this.deductLeaveTypeId || this.deductDays <= 0) return;

    this.leaveBalanceService
      .deductLeave(this.selectedUserIdForDeduct, this.deductLeaveTypeId, this.deductDays)
      .subscribe({
        next: () => this.toastr.success('Leave deducted successfully.', 'Success'),
        error: (err) => {
          const msg = err.error?.message || 'Failed to deduct leave.';
          this.toastr.error(msg, 'Error');
        }
      });
  }
}
