import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../services/auth-service';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule],
  selector: 'app-login',
  templateUrl: './login.html',
  styleUrls: ['./login.css']
})
export class Login{
  username = '';
  password = '';
  error = '';

  constructor(private auth: AuthService, private router: Router) {}

  login() {
  this.auth.login(this.username, this.password).subscribe({
    next: (res) => {
      this.auth.saveToken(res.accessToken);
      this.router.navigate(['/home']);
    },
    error: () => {
      this.error = 'Invalid credentials';
    }
  });
}
}
