import { LeaveBalanceResponseDto } from './leave-balance-response.model';
import { LeaveRequestResponse } from './leave-request-response.model';

export interface UserDetailDto {
  id: string;
  username: string;
  email: string;
  role: string;
  gender: string;
  isActive: boolean;
  lastLoginAt: string;
  createdAt: string;
  createdBy?: string;
  isDeleted: boolean;
  deletedAt?: string;

  leaveBalances: LeaveBalanceResponseDto[];
  leaveRequests: LeaveRequestResponse[];
}
