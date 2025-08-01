import { Component, signal } from '@angular/core';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UserService } from '../services/user.service';
import { MatButtonModule } from '@angular/material/button';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS, MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-login',
  imports: [
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './login.html',
  styleUrl: './login.css',
  providers: [
    {provide: MAT_FORM_FIELD_DEFAULT_OPTIONS, useValue: {appearance: 'outline'}}
  ]
})
export class Login {
  username : string = "";
  password : string = "";
  hidePassword = signal(true);

  fc = new FormControl(null);

  constructor(private userService : UserService ){}
  handleLogin(){
    this.userService.login(this.username, this.password);
  }
  passwordView(){
    this.hidePassword.set(!this.hidePassword());
  }
}
