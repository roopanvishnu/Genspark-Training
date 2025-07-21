import { Component, HostListener } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Header } from './components/header/header';
import { Sidebar } from './components/sidebar/sidebar';
import { AuthStateService } from './services/auth-state.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, Header, Sidebar, CommonModule],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App {
  isLoggedIn = false;
  sidebarOpen = false;
  isMobile = false;

  constructor(private authState: AuthStateService) {
    this.authState.isLoggedIn$.subscribe(status => this.isLoggedIn = status);
    this.checkMobileView();
  }

  toggleSidebar() {
    this.sidebarOpen = !this.sidebarOpen;
    if (this.isMobile) {
      document.body.style.overflow = this.sidebarOpen ? 'hidden' : '';
    }
  }

  @HostListener('window:resize')
  checkMobileView() {
    this.isMobile = window.innerWidth < 768;
    if (!this.isMobile) {
      this.sidebarOpen = false;
      document.body.style.overflow = '';
    }
  }
}
