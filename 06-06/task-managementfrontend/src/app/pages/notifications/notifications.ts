
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SignalRService } from '../../services/signal-r.service';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notifications.html',
  styleUrl: './notifications.css'
})
export class Notifications implements OnInit {
  messages: { message: string; type: string }[] = [];

  constructor(private signalRService: SignalRService) {}

  ngOnInit() {
    const connection = this.signalRService.createConnection();

    connection.on('taskCreated', data => {
      this.messages.unshift({
        message: `Task Created: ${data.title} (ID: ${data.taskId})`,
        type: 'success'
      });
    });

    connection.on('taskUpdated', data => {
      this.messages.unshift({
        message: `Task Updated: ID ${data.taskId}, Status: ${data.status}`,
        type: 'info'
      });
    });

    connection.on('taskAssigned', data => {
      this.messages.unshift({
        message: `Task Assigned to ${data.assignee} (ID: ${data.taskId})`,
        type: 'warning'
      });
    });

    connection.on('tasksBroadcast', data => {
      this.messages.unshift({
        message: `${data.assignedCount} users received broadcasted task: ${data.title}`,
        type: 'error'
      });
    });

    connection.start()
      .then(() => console.log('✅ SignalR connected for notifications'))
      .catch(err => console.error('❌ SignalR connection failed', err));
  }
}
