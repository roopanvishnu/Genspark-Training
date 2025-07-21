
import { Component } from '@angular/core';
import { CustomerDetailsComponent } from './customer-details/customer-details.component';
import { ProductListComponent, Product } from './product-list/product-list.component';
import { CartComponent, CartItem } from './cart/cart.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CustomerDetailsComponent, ProductListComponent, CartComponent],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App {
  cartItems: CartItem[] = [];
  cartCount = 0;
  title = 'simple-cart-app';

  addToCart(product: Product) {
    const existingItem = this.cartItems.find(item => item.id === product.id);
    
    if (existingItem) {
      existingItem.quantity++;
    } else {
      this.cartItems.push({
        id: product.id,
        name: product.name,
        quantity: 1
      });
    }
    this.cartCount++;
  }
}