import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

import { LeaveRequestDto } from '../models/leave-request.dto';
import { LeaveRequestResponse } from '../models/leave-request-response.model';
import { UpdateLeaveRequestStatusDto } from '../models/update-leave-status.dto';
import { ApiResponse } from '../models/api-response.model';
import { PaginationResponse } from '../models/pagination-response.model';

@Injectable({
  providedIn: 'root'
})
export class LeaveRequestService {
  private baseUrl = `${environment.apiUrl}/leave-requests`;

  constructor(private http: HttpClient) {}

  createLeaveRequest(dto: LeaveRequestDto): Observable<ApiResponse<LeaveRequestResponse>> {
    return this.http.post<ApiResponse<LeaveRequestResponse>>(this.baseUrl, dto);
  }

  getAllLeaveRequests(
    page = 1,
    pageSize = 10,
    searchTerm = '',
    status = '',
    sortBy = 'CreatedAt',
    sortOrder = 'asc'
  ): Observable<ApiResponse<PaginationResponse<LeaveRequestResponse>>> {
    const params = {
      page: page.toString(),
      pageSize: pageSize.toString(),
      searchTerm,
      status,
      sortBy,
      sortOrder
    };

    return this.http.get<ApiResponse<PaginationResponse<LeaveRequestResponse>>>(this.baseUrl, { params });
  }

  getLeaveRequestById(id: string): Observable<ApiResponse<LeaveRequestResponse>> {
    return this.http.get<ApiResponse<LeaveRequestResponse>>(`${this.baseUrl}/${id}`);
  }

  updateLeaveStatus(id: string, statusDto: UpdateLeaveRequestStatusDto): Observable<ApiResponse<null>> {
    return this.http.put<ApiResponse<null>>(`${this.baseUrl}/${id}/status`, statusDto);
  }

  cancelLeaveRequest(id: string): Observable<ApiResponse<null>> {
    return this.http.put<ApiResponse<null>>(`${this.baseUrl}/${id}/cancel`, {});
  }

  deleteLeaveRequest(id: string): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.baseUrl}/${id}`);
  }
}
