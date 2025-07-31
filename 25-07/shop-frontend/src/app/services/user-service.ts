import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface User {
  userId: number;
  username: string;
  password?: string; // Only used during create/edit
}

export interface UserDTO {
  username: string;
  password: string;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = 'http://localhost:5043/api/User';

  constructor(private http: HttpClient) {}

  getAll(): Observable<User[]> {
    return this.http.get<User[]>(this.apiUrl);
  }

  getById(id: number): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/Details/${id}`);
  }

  create(user: UserDTO): Observable<User> {
    return this.http.post<User>(`${this.apiUrl}/Create`, user);
  }

  update(id: number, user: UserDTO): Observable<User> {
    return this.http.post<User>(`${this.apiUrl}/Edit/${id}`, user);
  }

  delete(id: number): Observable<User> {
    return this.http.post<User>(`${this.apiUrl}/Delete/${id}`, {});
  }
  
}
