import { Component, inject } from '@angular/core';
import { ProductService } from '../service/product.service';
import { Product } from '../models/product'; 

@Component({
  selector: 'app-product',
  standalone: true,
  templateUrl: './product.html',
  styleUrls: ['./product.css']
})
export class ProductComponent {
  product: Product | null = null;

  private productService = inject(ProductService);

  constructor() {
    this.productService.getproduct(1).subscribe({
      next: (data) => {
        this.product = data; // ✅ Corrected property name
        console.log(data);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('all done');
      }
    });
  }
}
