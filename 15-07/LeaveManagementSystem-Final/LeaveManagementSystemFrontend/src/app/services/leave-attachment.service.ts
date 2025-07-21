import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LeaveAttachmentResponse } from '../models/leave-attachment.model';
import { ApiResponse } from '../models/api-response.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class LeaveAttachmentService {
  private baseUrl = `${environment.apiUrl}/leave-attachments`;

  constructor(private http: HttpClient) {}

  uploadAttachment(leaveRequestId: string, file: File): Observable<ApiResponse<LeaveAttachmentResponse>> {
    const formData = new FormData();
    formData.append('leaveRequestId', leaveRequestId);
    formData.append('file', file);

    return this.http.post<ApiResponse<LeaveAttachmentResponse>>(this.baseUrl, formData);
  }

  getAttachmentsByLeaveRequestId(leaveRequestId: string): Observable<ApiResponse<{ $values: LeaveAttachmentResponse[] }>> {
    return this.http.get<ApiResponse<{ $values: LeaveAttachmentResponse[] }>>(
      `${this.baseUrl}/by-leave-request/${leaveRequestId}`
    );
  }

  downloadAttachment(id: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/${id}`, { responseType: 'blob' });
  }

  deleteAttachment(id: string): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.baseUrl}/${id}`);
  }
}
