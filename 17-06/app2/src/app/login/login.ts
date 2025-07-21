import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Userservice } from '../services/userservice';
import { UserLoginModel } from '../models/userloginmodel';
import { Router } from '@angular/router';
@Component({
  selector: 'app-login',
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  user: UserLoginModel = new UserLoginModel();

  constructor(private userservice: Userservice, private route: Router) {}

   onSubmit() {
    console.log('Username:', this.user.username);
    console.log('Password:', this.user.password);
     this.userservice.validateUserLogin(this.user);
    setTimeout(() => {
      this.route.navigateByUrl('/products');
    }, 1000);
  }

  logout() {
    this.userservice.logout();
    this.route.navigateByUrl('/');
  }
}