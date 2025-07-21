import { Component, OnInit } from '@angular/core';
import { NotificationService, Notification } from '../../services/notification.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notification.html',
  styleUrls: ['./notification.css']
})
export class NotificationComponent implements OnInit {
  userId = localStorage.getItem('userId') || '';
  unreadNotifications: Notification[] = [];
  readNotifications: Notification[] = [];
  unreadCount = 0;
  loading = false;

  currentTab: 'unread' | 'read' = 'unread';
  page = 1;
  pageSize = 10;

  constructor(private notificationService: NotificationService) {}

  ngOnInit(): void {
    this.loadNotifications('unread');
  }

  switchTab(tab: 'unread' | 'read'): void {
    this.currentTab = tab;
    this.page = 1;

    if (tab === 'unread' && this.unreadNotifications.length === 0) {
      this.loadNotifications('unread');
    } else if (tab === 'read' && this.readNotifications.length === 0) {
      this.loadNotifications('read');
    }
  }

  
  loadNotifications(type: 'unread' | 'read'): void {
  if (!this.userId) return;

  this.loading = true;

  const serviceCall = type === 'unread'
    ? this.notificationService.getUnread(this.userId)
    : this.notificationService.getAll(this.userId);

  serviceCall.subscribe({
    next: (response) => {
      const data = response?.$values || [];

      if (type === 'unread') {
        this.unreadNotifications.push(...data);
        this.unreadCount = this.unreadNotifications.length;
      } else {
        const read = data.filter(n => n.isRead);
        this.readNotifications.push(...read);
      }

      this.loading = false;
    },
    error: (err) => {
      console.error('Error loading notifications:', err);
      this.loading = false;
    }
  });
}


  markAsRead(notification: Notification): void {
    if (notification.isRead) return;

    this.notificationService.markAsRead(notification.id).subscribe({
      next: () => {
        notification.isRead = true;
        this.unreadNotifications = this.unreadNotifications.filter(n => n.id !== notification.id);
        this.readNotifications.unshift(notification);
        this.unreadCount--;
      },
      error: (err) => {
        console.error('Failed to mark as read:', err);
      }
    });
  }

  onScroll(event: Event): void {
    const el = event.target as HTMLElement;
    const threshold = 100;
    const bottomReached = el.scrollHeight - el.scrollTop <= el.clientHeight + threshold;

    if (bottomReached && !this.loading) {
      this.loadMore();
    }
  }

  loadMore(): void {
    // For real backend pagination, you’d increment page and pass it in query
    this.page++;
    this.loadNotifications(this.currentTab);
  }
}
