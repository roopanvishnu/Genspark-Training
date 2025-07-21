import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  templateUrl: './login.html',
  imports : [CommonModule,FormsModule],
  styleUrls: ['./login.scss']
})
export class Login {
  username = '';
  password = '';

  constructor(private router: Router) {}

  submit() {
    if (this.username === 'test' && this.password === '1234') {
      localStorage.setItem('token', 'mock-token');
      this.router.navigate(['/products']);
    } else {
      alert('Invalid credentials');
    }
  }
}
