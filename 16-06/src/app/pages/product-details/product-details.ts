import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  templateUrl: './product-details.html'
})
export class ProductDetails implements OnInit {
  product: any = null;
  error: boolean = false;

  constructor(private route: ActivatedRoute, private http: HttpClient) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    this.http.get(`https://dummyjson.com/products/${id}`).subscribe({
      next: (res) => this.product = res,
      error: () => this.error = true
    });
  }
}
