export interface LeaveRequestResponse {
  id: string;
  userId: string;
  userName: string;

  leaveTypeId: string;
  leaveTypeName: string;

  startDate: string;
  endDate: string;
  reason: string;
  status: string;

  reviewedById?: string | null;
  reviewedByName?: string | null;

  updatedAt: string;
  createdAt: string;
}
