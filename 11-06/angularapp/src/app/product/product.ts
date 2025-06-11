import { CurrencyPipe, NgIf } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ProductModel } from '../models/productmodel';
import { ProductService } from '../product.service.ts/productservice';

@Component({
  selector: 'app-product',
  standalone: true,
  templateUrl: './product.html',
  styleUrls: ['./product.css'],
  imports: [CurrencyPipe, NgIf] // ✅ Import pipes and directives you use
})
export class Product {
  product: ProductModel | null = new ProductModel();
  private productService = inject(ProductService);

  constructor() {
    this.productService.getProduct(1).subscribe({
      next: (data) => {
        this.product = data as ProductModel;
        console.log(this.product);
      },
      error: (err) => console.log(err),
      complete: () => console.log("All done")
    });
  }
}
