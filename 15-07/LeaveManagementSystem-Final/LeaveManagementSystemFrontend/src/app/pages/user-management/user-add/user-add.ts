import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../../../services/user.service';
import { CreateUserDto } from '../../../models/user-dto.model';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-user-add',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-add.html',
  styleUrls: ['./user-add.css']
})
export class UserAdd {
  userForm: CreateUserDto = {
    username: '',
    email: '',
    password: '',
    role: 'Employee',
    gender: 'Male',
    isActive: true
  };

  isLoading = false;

  constructor(
    private userService: UserService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  saveUser(form: NgForm) {
    if (form.invalid) return;

    this.isLoading = true;

    this.userService.createUser(this.userForm).subscribe({
      next: () => {
        this.toastr.success('User created successfully!', 'Success');
        this.router.navigate(['/users']);
        this.isLoading = false;
      },
      error: err => {
        console.error('Create User Failed:', err);
        const msg = err?.error?.message || 'Failed to create user';
        this.toastr.error(msg, 'Error');
        this.isLoading = false;
      }
    });
  }
}
