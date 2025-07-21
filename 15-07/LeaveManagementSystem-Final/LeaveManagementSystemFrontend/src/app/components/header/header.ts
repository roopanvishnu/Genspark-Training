import {
  Component,
  ElementRef,
  HostListener,
  OnInit,
  ViewChild,
  Output,
  EventEmitter
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthStateService } from '../../services/auth-state.service';
import { NotificationService, Notification } from '../../services/notification.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './header.html',
  styleUrls: ['./header.css'],
})
export class Header implements OnInit {
  @Output() toggleSidebar = new EventEmitter<void>();
  isLoggedIn: boolean = false;
  username: string = 'Guest';
  role: string | null = null;

  showNotifications = false;
  notifications: Notification[] = [];
  unreadCount = 0;
  loading = false;

  @ViewChild('notificationWrapper') notificationWrapper!: ElementRef;

  constructor(
    private router: Router,
    private authState: AuthStateService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.authState.isLoggedIn$.subscribe((status) => (this.isLoggedIn = status));
    this.authState.username$.subscribe((name) => {
      this.username = name || 'Guest';
    });
    this.authState.role$.subscribe((r) => {
      this.role = r;
    });

    this.fetchUnreadCount();
  }

  toggleNotifications() {
    this.showNotifications = !this.showNotifications;
    if (this.showNotifications) {
      const userId = localStorage.getItem('userId') || '';
      if (!userId) return;

      this.loading = true;
      this.notificationService.getAll(userId).subscribe({
        next: (data) => {
          this.notifications = data.$values || [];
          this.loading = false;
        },
        error: () => (this.loading = false),
      });
    }
  }

  markAllAsRead() {
    const userId = localStorage.getItem('userId') || '';
    if (!userId) return;

    this.notificationService.markAllAsRead(userId).subscribe(() => {
      this.notifications = this.notifications.map((n) => ({
        ...n,
        isRead: true,
        readAt: new Date().toISOString(),
      }));
      this.fetchUnreadCount(); 
    });
  }

  fetchUnreadCount() {
    const userId = localStorage.getItem('userId') || '';
    if (!userId) return;

    this.notificationService.getUnread(userId).subscribe({
      next: (data) => {
        this.unreadCount = data?.$values?.length || 0;
      },
      error: () => (this.unreadCount = 0),
    });
  }

  logout() {
    this.authState.logout();
    this.router.navigate(['/login']);
  }

  login() {
    this.router.navigate(['/login']);
  }

  @HostListener('document:click', ['$event'])
  handleClickOutside(event: MouseEvent) {
    if (
      this.showNotifications &&
      this.notificationWrapper &&
      !this.notificationWrapper.nativeElement.contains(event.target)
    ) {
      this.showNotifications = false;
    }
  }
}
