import { Component } from '@angular/core';

@Component({
  selector: 'app-customer-details',
  template: `
    <h2>Customer Details</h2>
    <p>Name: Roopan Vishnu</p>
    <p>Email: roopanvishnu.work&#64;gmail.com</p>
    <p>Address: Nanganallur Chennai</p>

    <button (click)="like()">👍</button>
    <span>{{ likeCount }}</span>
    <button (click)="dislike()">👎</button> 
    <span>{{ dislikeCount }}</span>
  `
})
export class CustomerDetailsComponent {
  likeCount = 0;
  dislikeCount = 0;

  like() {
    this.likeCount++;
  }

  dislike() {
    this.dislikeCount++;
  }
}
