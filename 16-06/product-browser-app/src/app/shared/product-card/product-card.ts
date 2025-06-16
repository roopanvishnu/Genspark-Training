import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-product-card',
  templateUrl: './product-card.html',
  styleUrls: ['./product-card.scss']
})
export class ProductCard {
  @Input() product: any;
  @Input() search = '';

  highlight(title: string): string {
    if (!this.search) return title;
    const regex = new RegExp(`(${this.search})`, 'gi');
    return title.replace(regex, '<mark>$1</mark>');
  }
}
