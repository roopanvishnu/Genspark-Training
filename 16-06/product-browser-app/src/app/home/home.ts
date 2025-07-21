import { Component, HostListener, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Productservice } from '../services/productservice';
import { ProductModel } from '../models/ProductModel';
import { Product } from '../product/product';
import { CommonModule } from '@angular/common';
import {
  debounceTime,
  distinctUntilChanged,
  Subject,
  switchMap,
  tap,
} from 'rxjs';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [FormsModule, Product, CommonModule],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home implements OnInit {
  products: ProductModel[] = [];
  searchTerm: string = '';
  searchSubject = new Subject<string>();
  loading: boolean = false;
  limit = 10;
  skip = 0;
  isFetching = false;
  isEnd = false;

  constructor(private productService: Productservice) {}

  ngOnInit() {
    this.loadMoreProducts();

    this.searchSubject
      .pipe(
        debounceTime(2000),
        distinctUntilChanged(),
        tap(() => {
          this.loading = true;
          this.skip = 0;
          this.isEnd = false;
        }),
        switchMap((query) =>
          this.productService.getPaginatedProducts(this.limit, this.skip, query)
        ),
        tap(() => (this.loading = false))
      )
      .subscribe((data: any) => {
        this.products = data.products;
      });
  }

  handleSearchProducts() {
    this.searchSubject.next(this.searchTerm);
  }

  loadMoreProducts() {
    if (this.loading || this.isEnd) return;

    this.loading = true;
    this.productService.getPaginatedProducts(this.limit, this.skip,this.searchTerm).subscribe({
      next: (data: any) => {
        this.products = [...this.products, ...data.products];
        this.skip += this.limit;
        if (data.products.length < this.limit) {
          this.isEnd = true;
        }
        this.loading = false;
      },
      error: () => (this.loading = false),
    });
  }


  @HostListener('window:scroll', [])
  onScroll(): void {
    const scrollPosition = window.innerHeight + window.scrollY;
    const bottom = document.body.offsetHeight - 100;
    if (scrollPosition >= bottom) {
      this.loadMoreProducts();
    }
  }
}

