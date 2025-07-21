
import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface CartItem {
  id: number;
  name: string;
  quantity: number;
}

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule],
  template: `
    <h3>🛒 Shopping Cart</h3>
    <p><strong>Total Items: {{ totalCount }}</strong></p>
    
    <div *ngIf="items.length === 0" class="cart-empty">
      Your cart is empty
    </div>
    
    <div *ngFor="let item of items" class="cart-item">
      <span>{{ item.name }}</span>
      <span class="quantity">× {{ item.quantity }}</span>
    </div>
  `
})
export class CartComponent {
  @Input() items: CartItem[] = [];
  @Input() totalCount = 0;
}