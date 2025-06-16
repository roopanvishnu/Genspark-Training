import { Component, OnInit, HostListener } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { CommonModule } from '@angular/common';

import { ProductCard } from '../../shared/product-card/product-card';
import { LoadingSpinner } from '../../shared/loading-spinner/loading-spinner';
import { BackToTop } from '../../shared/back-to-top/back-to-top';

@Component({
  selector: 'app-home',
  standalone: true,
  templateUrl: './home.html',
  styleUrls: ['./home.scss'],
  imports: [
    CommonModule,
    ProductCard,
    LoadingSpinner,
    BackToTop
  ]
})
export class Home implements OnInit {
  products: any[] = [];
  searchTerm$ = new Subject<string>();
  isLoading = false;
  skip = 0;
  query = '';
  showBackToTop = false;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.searchTerm$
      .pipe(
        debounceTime(400),
        distinctUntilChanged(),
        switchMap((term) => {
          this.skip = 0;
          this.products = [];
          this.query = term;
          return this.fetchProducts(term, this.skip);
        })
      )
      .subscribe((res: any) => {
        this.products = res.products;
      });

    this.loadInitialProducts();
  }

  fetchProducts(query: string, skip: number) {
    this.isLoading = true;
    return this.http
      .get(`https://dummyjson.com/products/search?q=${query}&limit=10&skip=${skip}`)
      .pipe(tap(() => (this.isLoading = false)));
  }

  loadInitialProducts() {
    this.fetchProducts(this.query, this.skip).subscribe((res: any) => {
      this.products = res.products;
    });
  }

  loadMore() {
    if (this.isLoading) return;
    this.skip += 10;
    this.fetchProducts(this.query, this.skip).subscribe((res: any) => {
      this.products = [...this.products, ...res.products];
    });
  }

  onSearchInput(event: Event) {
    const input = event.target as HTMLInputElement;
    this.searchTerm$.next(input.value);
  }

  @HostListener('window:scroll', [])
  onWindowScroll() {
    const pos = window.scrollY + window.innerHeight;
    const max = document.documentElement.scrollHeight;

    if (pos > max - 100) this.loadMore();

    this.showBackToTop = window.scrollY > 300;
  }

  scrollToTop() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
}
