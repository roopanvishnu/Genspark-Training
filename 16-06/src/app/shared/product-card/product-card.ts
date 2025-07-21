import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-product-card',
  standalone: true,
  templateUrl: './product-card.html',
  styleUrls: ['./product-card.scss']
})
export class ProductCard {
  @Input() product: any;
  @Input() search = '';

  constructor(private router: Router) {}

  highlight(title: string): string {
    if (!this.search) return title;
    const regex = new RegExp(`(${this.search})`, 'gi');
    return title.replace(regex, '<mark>$1</mark>');
  }

  goToDetails() {
    this.router.navigate(['/home', this.product.id]);
  }
}
