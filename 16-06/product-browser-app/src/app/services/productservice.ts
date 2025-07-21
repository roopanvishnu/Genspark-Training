import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class Productservice {
  private http = inject(HttpClient);

  getallProducts(): Observable<any> {
    return this.http.get<any[]>('https://dummyjson.com/products');
  }

  getProductSearchResult(searchData: string) {
    return this.http.get(
      'https://dummyjson.com/products/search?q=' + searchData
    );
  }
  getPaginatedProducts(limit: number, skip: number, query: string): Observable<any> {
    const url = `https://dummyjson.com/products/search?q=${query}&limit=${limit}&skip=${skip}&select=title,price,thumbnail,description`;
    return this.http.get(url);
  }

  constructor() {}
}
