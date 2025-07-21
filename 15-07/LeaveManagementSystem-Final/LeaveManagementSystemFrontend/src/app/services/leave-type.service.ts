import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { LeaveType } from '../models/leave-type.model';
import { ApiResponse } from '../models/api-response.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class LeaveTypeService {
  private readonly apiUrl = `${environment.apiUrl}/leavetypes`;

  constructor(private http: HttpClient) {}

  getLeaveTypes(): Observable<LeaveType[]> {
    return this.http
      .get<ApiResponse<{ $values: LeaveType[] }>>(this.apiUrl)
      .pipe(map(res => res.data.$values)); // handle .NET $values wrapper
  }

  getLeaveTypeById(id: string): Observable<LeaveType> {
    return this.http
      .get<ApiResponse<LeaveType>>(`${this.apiUrl}/${id}`)
      .pipe(map(res => res.data));
  }

  addLeaveType(dto: LeaveType): Observable<LeaveType> {
    return this.http
      .post<ApiResponse<LeaveType>>(this.apiUrl, dto)
      .pipe(map(res => res.data));
  }

  updateLeaveType(id: string, dto: LeaveType): Observable<LeaveType> {
    return this.http
      .put<ApiResponse<LeaveType>>(`${this.apiUrl}/${id}`, dto)
      .pipe(map(res => res.data));
  }

  deleteLeaveType(id: string): Observable<void> {
    return this.http
      .delete<ApiResponse<null>>(`${this.apiUrl}/${id}`)
      .pipe(map(() => {}));
  }
}
