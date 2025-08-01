import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ContactU {
  id: number;
  name: string;
  email: string;
  phone: string;
  content: string;
}

export interface ContactUsAddDTO {
  name: string;
  email: string;
  phone: string;
  content: string;
}

@Injectable({
  providedIn: 'root'
})
export class ContactusService {
  private baseUrl = 'http://localhost:5043/api/ContactUs';

  constructor(private http: HttpClient) {}

  getAll(): Observable<ContactU[]> {
    return this.http.get<ContactU[]>(this.baseUrl);
  }

  create(contact: ContactUsAddDTO): Observable<ContactU> {
    return this.http.post<ContactU>(`${this.baseUrl}/Create`, contact);
  }

  update(id: number, contact: ContactUsAddDTO): Observable<ContactU> {
    return this.http.post<ContactU>(`${this.baseUrl}/Edit/${id}`, contact);
  }

  delete(id: number): Observable<ContactU> {
    return this.http.post<ContactU>(`${this.baseUrl}/Delete/${id}`, {});
  }
}
