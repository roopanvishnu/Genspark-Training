import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterModule, Router } from '@angular/router';
import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { AuthService } from './services/auth.service';
import { SignalRService } from './services/signalr.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterModule],
  templateUrl : 'main.html'
})
export class App implements OnInit {
  currentUser = this.authService.getCurrentUser();
  notifications: string[] = [];
  showNotifications = false;
  showUserMenu = false;

  constructor(
    public authService: AuthService,
    private signalRService: SignalRService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Subscribe to user changes
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
      if (!user) {
        this.router.navigate(['/login']);
      }
    });

    // Subscribe to notifications
    this.signalRService.notifications$.subscribe(notifications => {
      this.notifications = notifications;
    });

    // Close dropdowns when clicking outside
    document.addEventListener('click', (event) => {
      if (!event.target) return;
      const target = event.target as HTMLElement;
      if (!target.closest('.relative')) {
        this.showNotifications = false;
        this.showUserMenu = false;
      }
    });

    // Redirect to login if not authenticated
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/login']);
    }
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  clearNotifications(): void {
    this.signalRService.clearNotifications();
    this.showNotifications = false;
  }
}

bootstrapApplication(App, {
  providers: [
    provideRouter(routes)
  ]
});