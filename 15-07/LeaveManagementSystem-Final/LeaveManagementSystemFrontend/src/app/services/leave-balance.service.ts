import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import {
  UserLeaveBalanceResponseDto,
  UserLeaveBalanceForTypeResponseDto
} from '../models/user-leave-balance.model';

@Injectable({ providedIn: 'root' })
export class LeaveBalanceService {
  private readonly baseUrl = `${environment.apiUrl}/leave-balance`;

  constructor(private http: HttpClient) {}

  getLeaveBalance(userId: string): Observable<ApiResponse<UserLeaveBalanceResponseDto>> {
    return this.http.get<ApiResponse<UserLeaveBalanceResponseDto>>(
      `${this.baseUrl}/user/${userId}`
    );
  }

  getLeaveBalanceForType(userId: string, leaveTypeId: string): Observable<ApiResponse<UserLeaveBalanceForTypeResponseDto>> {
    return this.http.get<ApiResponse<UserLeaveBalanceForTypeResponseDto>>(
      `${this.baseUrl}/user/${userId}/leaveType/${leaveTypeId}`
    );
  }

  initializeLeaveBalance(userId: string): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(
      `${this.baseUrl}/initialize/user/${userId}`, {}
    );
  }

  initializeLeaveType(userId: string, leaveTypeId: string, standardLeaveCount: number): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(
      `${this.baseUrl}/initialize/user/${userId}/leaveType/${leaveTypeId}?standardLeaveCount=${standardLeaveCount}`, {}
    );
  }

  deductLeave(userId: string, leaveTypeId: string, days: number): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(
      `${this.baseUrl}/deduct?userId=${userId}&leaveTypeId=${leaveTypeId}&days=${days}`, {}
    );
  }

  resetLeaveBalance(userId: string): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(
      `${this.baseUrl}/reset/user/${userId}`, {}
    );
  }
}
