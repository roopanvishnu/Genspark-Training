
import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface Product {
  id: number;
  name: string;
  image: string;
}

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <h2>Products</h2>
    <div *ngFor="let product of products">
      <p>{{ product.name }}</p>
      <img [src]="product.image" width="100" />
      <button (click)="addToCart(product)">Add to Cart</button>
      <hr />
    </div>
  `
})
export class ProductListComponent {
  @Output() productAdded = new EventEmitter<Product>();

  products: Product[] = [
    { 
      id: 1, 
      name: 'Leather Boots', 
      image: 'https://images.unsplash.com/photo-1605812860427-4024433a70fd?q=80&w=3456&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D' 
    },
    { 
      id: 2, 
      name: 'Key chains', 
      image: 'https://images.unsplash.com/photo-1674660638936-c0005c862a0d?q=80&w=3870&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D' 
    },
    { 
      id: 3, 
      name: 'Men Wallet', 
      image: 'https://images.unsplash.com/photo-1703355685576-ec1c499ab6da?w=900&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8N3x8bWVuJTIwd2FsbGV0fGVufDB8fDB8fHww' 
    }
  ];

  addToCart(product: Product) {
    this.productAdded.emit(product);
  }
}