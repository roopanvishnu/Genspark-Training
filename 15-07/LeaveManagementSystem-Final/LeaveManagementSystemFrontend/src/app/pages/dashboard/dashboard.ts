import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthStateService } from '../../services/auth-state.service';

import { HrDashboardComponent } from '../hr-dashboard/hr-dashboard';
import { EmployeeDashboardComponent } from '../employee-dashboard/employee-dashboard';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css'],
  imports: [CommonModule, HrDashboardComponent, EmployeeDashboardComponent],
})
export class Dashboard implements OnInit {
  private authState = inject(AuthStateService);
  role: string | null = null;
  isHR = false;
  isEmployee = false;

  ngOnInit(): void {
    this.authState.role$.subscribe((role) => {
      this.role = role;
      this.isHR = role === 'HR';
      this.isEmployee = role === 'Employee';
    });
  }
}
