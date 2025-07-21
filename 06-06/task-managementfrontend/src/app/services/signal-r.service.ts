// signalr.service.ts
import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';

@Injectable({ providedIn: 'root' })
export class SignalRService {
  createConnection(): signalR.HubConnection {
    return new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7120/hubs/tasks', { withCredentials: true })
      .build();
  }
}
