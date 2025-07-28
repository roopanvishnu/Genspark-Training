export interface LeaveRequestDto {
  leaveTypeId: string;
  startDate: string;
  endDate: string;
  reason: string;
  role : string;
  status?: string; // optional, backend defaults to "Pending"
}
