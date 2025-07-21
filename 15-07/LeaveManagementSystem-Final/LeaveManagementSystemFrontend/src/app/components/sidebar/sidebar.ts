import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AuthStateService } from '../../services/auth-state.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterModule, CommonModule],
  templateUrl: './sidebar.html',
  styleUrls: ['./sidebar.css']
})
export class Sidebar implements OnInit {
  isHR: boolean = false;
  isLoggedIn: boolean = false;
  private sub: Subscription = new Subscription();

  @Input() sidebarOpen: boolean = false;
  @Input() isMobile: boolean = false;
  @Output() closeSidebar = new EventEmitter<void>();

  constructor(private authState: AuthStateService) {}

  ngOnInit(): void {
    this.sub.add(this.authState.isLoggedIn$.subscribe(status => this.isLoggedIn = status));
    this.sub.add(this.authState.role$.subscribe(role => this.isHR = (role === 'HR')));
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
