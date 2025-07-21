
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { CustomValidators } from '../../validators/custom-validators';
import { UserService } from '../../services/user.service';
import { UserModel } from '../../models/user.model';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './user-form.html',
  styleUrls: ['./user-form.scss']
})
export class UserForm {
  form: FormGroup;
  roles = ['Admin', 'User', 'Guest'];

  constructor(private fb: FormBuilder, private userService: UserService, private router: Router) {
    this.form = this.fb.group({
      username: ['', [Validators.required, CustomValidators.bannedUsernames(['admin', 'root'])]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, CustomValidators.passwordStrenght()]],
      confirmPassword: ['', Validators.required],
      role: ['User', Validators.required],
    }, {
      validators: CustomValidators.matchPassword('password', 'confirmPassword')
    });
  }

  get f() {
    return this.form.controls;
  }

  onSubmit() {
    if (this.form.invalid) return;
    const { username, email, password, role } = this.form.value;
    const newUser: UserModel = { username, email, password, role };
    this.userService.addUser(newUser);
    alert('User added successfully!');
    this.form.reset({ role: 'User' });
    setTimeout(() => this.router.navigate(['/users']), 0);
  }
}
