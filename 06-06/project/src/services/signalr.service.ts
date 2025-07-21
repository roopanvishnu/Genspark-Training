import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection?: HubConnection;
  private notificationsSubject = new BehaviorSubject<string[]>([]);
  public notifications$ = this.notificationsSubject.asObservable();

  constructor() {
    this.startConnection();
  }

  private startConnection(): void {
    // Mock SignalR connection - replace with actual hub URL
    console.log('SignalR connection started (mock)');
    
    // Simulate real-time notifications
    this.simulateNotifications();
  }

  private simulateNotifications(): void {
    setInterval(() => {
      const notifications = [
        'New task assigned to you',
        'Task status updated',
        'New comment added',
        'File uploaded to task',
        'Task deadline approaching'
      ];
      
      const randomNotification = notifications[Math.floor(Math.random() * notifications.length)];
      const current = this.notificationsSubject.value;
      this.notificationsSubject.next([randomNotification, ...current.slice(0, 4)]);
    }, 30000); // Every 30 seconds
  }

  clearNotifications(): void {
    this.notificationsSubject.next([]);
  }

  disconnect(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }
}