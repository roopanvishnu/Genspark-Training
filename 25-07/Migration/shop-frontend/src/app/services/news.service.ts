import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface NewsItem {
  newsId: number;
  title: string;
  shortDescription: string;
  content: string;
  image: string;
  createdDate: string;
  status: number;
  userId: number;
}

export interface NewsAddDTO {
  title: string;
  shortDescription: string;
  content: string;
  image: string;
  createdDate: string;
  status: number;
  userId: number;
}

@Injectable({
  providedIn: 'root'
})
export class NewsService {
  private baseUrl = 'http://localhost:5043/api';

  constructor(private http: HttpClient) {}

  // Public
  getPublicNews(): Observable<NewsItem[]> {
    return this.http.get<NewsItem[]>(`${this.baseUrl}/News`);
  }

  // Authenticated
  getAll(): Observable<NewsItem[]> {
    return this.http.get<NewsItem[]>(`${this.baseUrl}/NewsManagement`);
  }

  create(news: NewsAddDTO): Observable<NewsItem> {
    return this.http.post<NewsItem>(`${this.baseUrl}/NewsManagement/Create`, news);
  }

  update(id: number, news: NewsAddDTO): Observable<NewsItem> {
    return this.http.post<NewsItem>(`${this.baseUrl}/NewsManagement/Edit/${id}`, news);
  }

  delete(id: number): Observable<NewsItem> {
    return this.http.post<NewsItem>(`${this.baseUrl}/NewsManagement/Delete/${id}`, {});
  }

  exportCSV(): Observable<Blob> {
    return this.http.post(`${this.baseUrl}/NewsManagement/Export`, {}, { responseType: 'blob' });
  }
}
