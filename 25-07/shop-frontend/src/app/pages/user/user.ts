import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService, User, UserDTO } from '../../services/user-service';

@Component({
  standalone: true,
  selector: 'app-user',
  imports: [CommonModule, FormsModule],
  templateUrl: './user.html',
  styleUrls: ['./user.css']
})
export class UserComponent implements OnInit {
  users: User[] = [];
  newUser: UserDTO = { username: '', password: '' };

  constructor(private userService: UserService) {}

  ngOnInit() {
    this.fetchUsers();
  }

  fetchUsers() {
    this.userService.getAll().subscribe(res => this.users = res);
  }
  

  createUser() {
    this.userService.create(this.newUser).subscribe(() => {
      this.newUser = { username: '', password: '' };
      this.fetchUsers();
    });
  }

  deleteUser(id: number) {
    this.userService.delete(id).subscribe(() => this.fetchUsers());
  }
  loadUsers() {
    this.userService.getAll().subscribe({
      next: (res) => this.users = res,
      error: (err) => console.error('Error loading users', err)
    });
  }
}
