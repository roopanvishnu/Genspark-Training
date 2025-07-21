export interface LeaveBalanceResponseDto {
  leaveTypeId: string;
  leaveTypeName: string;
  totalLeaves: number;
  usedLeaves: number;
  remainingLeaves: number;
}
