export interface LeaveRequestDto {
  leaveTypeId: string;
  startDate: string;
  endDate: string;
  reason: string;
  status?: string; // optional, backend defaults to "Pending"
}
