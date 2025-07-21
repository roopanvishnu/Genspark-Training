import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { RegisterRequest, UserRole } from '../models/user.model';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl : './register.component.html'
})
export class RegisterComponent {
  registerRequest: RegisterRequest = {
    email: '',
    password: '',
    firstName: '',
    lastName: '',
    role: UserRole.TeamMember
  };
  loading = false;
  error = '';
  UserRole = UserRole;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onSubmit(): void {
    if (!this.registerRequest.email || !this.registerRequest.password || 
        !this.registerRequest.firstName || !this.registerRequest.lastName) {
      this.error = 'Please fill in all fields';
      return;
    }

    this.loading = true;
    this.error = '';

    this.authService.register(this.registerRequest).subscribe({
      next: (response) => {
        this.loading = false;
        this.router.navigate(['/dashboard']);
      },
      error: (error) => {
        this.loading = false;
        this.error = error.message || 'Registration failed';
      }
    });
  }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }
}