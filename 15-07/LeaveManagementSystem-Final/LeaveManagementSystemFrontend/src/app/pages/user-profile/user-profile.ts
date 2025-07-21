import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../services/user.service';
import { UpdateUserDto } from '../../models/user-dto.model';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-profile.html',
  styleUrls: ['./user-profile.css']
})
export class UserProfile implements OnInit {
  user: any = null;
  updatedUser: UpdateUserDto = {
    username: '',
    email: '',
    gender: 'Other',
    role: 'Employee',
    isActive: true
  };

  isModalOpen = false;
  errorMsg = '';
  isLoading = false;

  changePwdDto = {
    email: '',
    currentPassword: '',
    newPassword: '',
    confirmNewPassword: ''
  };

  isPwdModalOpen = false;
  pwdError = '';
  isPwdSubmitting = false;

  constructor(private http: HttpClient, private userService: UserService,private toastr: ToastrService) {}

  ngOnInit(): void {
    this.fetchUserProfile();
  }

  fetchUserProfile() {
    this.isLoading = true;
    const token = localStorage.getItem('accessToken');
    const headers = new HttpHeaders({ 'Authorization': `Bearer ${token}` });

    this.http.get('http://localhost:5000/api/v1/auth/me', { headers }).subscribe({
      next: (res: any) => {
        this.user = res;
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMsg = 'Failed to load profile.';
        this.isLoading = false;
        console.error(err);
      }
    });
  }

  openEditModal() {
    this.updatedUser = { ...this.user }; // clone
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
  }

  saveChanges() {
    if (!this.user?.id || !this.updatedUser) return;

    this.userService.updateUser(this.user.id, this.updatedUser).subscribe({
      next: (res) => {
        this.user = res.data;
        this.toastr.success('Profile updated successfully', 'Success');
        this.closeModal();
      },
      error: (err) => {
        console.error(err);
        const msg = err?.error?.message || 'Failed to update profile.';
        this.toastr.error(msg, 'Update Failed');
      }
    });
  }

  openChangePwdModal() {
    this.changePwdDto.email = this.user?.email || '';
    this.changePwdDto.currentPassword = '';
    this.changePwdDto.newPassword = '';
    this.changePwdDto.confirmNewPassword = '';
    this.isPwdModalOpen = true;
    this.pwdError = '';
  }

  closePwdModal() {
    this.isPwdModalOpen = false;
    this.pwdError = '';
  }

  submitChangePassword() {
    this.pwdError = '';
    const { currentPassword, newPassword, confirmNewPassword } = this.changePwdDto;

    if (newPassword !== confirmNewPassword) {
      this.pwdError = 'New password and confirm password do not match.';
      return;
    }

    const token = localStorage.getItem('accessToken');
    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });

    this.isPwdSubmitting = true;
    this.http.post('http://localhost:5000/api/v1/Users/change-password', this.changePwdDto, { headers })
      .subscribe({
        next: () => {
          alert('Password changed successfully!');
          this.closePwdModal();
        },
        error: (err) => {
          this.pwdError = err.error?.message || 'Failed to change password.';
        },
        complete: () => this.isPwdSubmitting = false
      });
  }
}
