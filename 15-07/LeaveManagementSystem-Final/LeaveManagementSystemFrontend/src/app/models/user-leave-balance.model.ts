import { LeaveBalanceResponseDto } from './leave-balance-response.model';

export interface UserLeaveBalanceResponseDto {
  userId: string;
  userName: string;
  leaveBalances: LeaveBalanceResponseDto[];
}

export interface UserLeaveBalanceForTypeResponseDto {
  userId: string;
  userName: string;
  leaveBalance: LeaveBalanceResponseDto;
}
