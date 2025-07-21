import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private readonly api = 'https://localhost:7120/api/v1/users';

  constructor(private http: HttpClient) {}

getTeamMembers(): Observable<any> {
  return this.http.get<any>(`${this.api}?page=1&pageSize=100`);
}
getUsers(page: number, pageSize: number): Observable<any> {
    return this.http.get<any>(`${this.api}?page=${page}&pageSize=${pageSize}`);
  }
  updateUser(id: string, data: { fullName: string; role: string }): Observable<any> {
    return this.http.put(`${this.api}/${id}`, data);
  }
  deleteUser(userId: string) {
  const url = `https://localhost:7120/api/v1/users/delete/${userId}`;
  return this.http.delete(url);
}
}
