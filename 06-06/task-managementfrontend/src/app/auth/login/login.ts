import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl:'./login.html',
  styleUrl: './login.css'
})
export class Login {
  email = '';
  password = '';
  error = '';

  constructor(private auth: AuthService, private router: Router) {}

  onSubmit() {
    this.error = '';
    console.log('Logging in:', this.email, this.password);

    this.auth.login({ email: this.email, password: this.password }).subscribe({
      next: () => {
        console.log('Login successful — navigating to home');
        this.router.navigate(['/home']);
      },
      error: err => {
        this.error = err.error?.message || 'Login failed';
        console.error('Login error:', err);
      }
    });
  }
}
