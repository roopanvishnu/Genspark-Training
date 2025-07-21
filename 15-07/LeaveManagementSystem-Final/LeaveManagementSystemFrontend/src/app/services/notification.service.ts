import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Notification {
  id: string;
  message: string;
  createdAt: string;
  isRead: boolean;
  recipientId: string;
  readAt?: string;
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private baseUrl = `${environment.apiUrl}/notifications`;

  constructor(private http: HttpClient) {}

  getAll(userId: string): Observable<{ $values: Notification[] }> {
    return this.http.get<{ $values: Notification[] }>(`${this.baseUrl}/all/${userId}`);
  }

  getUnread(userId: string): Observable<{ $values: Notification[] }> {
    return this.http.get<{ $values: Notification[] }>(`${this.baseUrl}/unread/${userId}`);
  }

  markAsRead(id: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/mark-as-read/${id}`, {});
  }

  markAllAsRead(userId: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/mark-all-as-read/${userId}`, {});
  }
}
