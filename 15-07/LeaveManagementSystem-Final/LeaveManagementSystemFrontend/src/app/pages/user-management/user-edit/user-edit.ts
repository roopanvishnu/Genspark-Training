import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../../../services/user.service';
import { UpdateUserDto } from '../../../models/user-dto.model';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-user-edit',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-edit.html',
  styleUrls: ['./user-edit.css']
})
export class UserEdit implements OnInit {
  userForm: UpdateUserDto = {
    username: '',
    email: '',
    role: 'Employee',
    gender: 'Male',
    isActive: true
  };

  userId!: string;
  isLoading = false;

  constructor(
    private userService: UserService,
    private route: ActivatedRoute,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.userId = this.route.snapshot.paramMap.get('id')!;
    this.userService.getUserById(this.userId).subscribe({
      next: res => {
        const userData = res?.data;
        if (userData) {
          this.userForm = {
            username: userData.username,
            email: userData.email,
            role: userData.role,
            gender: userData.gender,
            isActive: userData.isActive
          };
        }
      },
      error: err => {
        console.error('Failed to fetch user:', err);
        this.toastr.error('Failed to load user data', 'Error');
      }
    });
  }

  saveUser(form: NgForm) {
    if (form.invalid) return;

    this.isLoading = true;

    this.userService.updateUser(this.userId, this.userForm).subscribe({
      next: () => {
        this.toastr.success('User updated successfully!', 'Success');
        this.isLoading = false;
      },
      error: err => {
        console.error('Update failed:', err);
        const msg = err?.error?.message || 'Failed to update user';
        this.toastr.error(msg, 'Update Failed');
        this.isLoading = false;
      }
    });
  }
}
