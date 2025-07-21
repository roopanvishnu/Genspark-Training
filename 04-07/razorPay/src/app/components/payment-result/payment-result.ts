import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-payment-result',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './payment-result.html',
  styleUrl: './payment-result.css'
})
export class PaymentResult implements OnInit {
  paymentHistory: any[] = [];

  ngOnInit(): void {
    const data = localStorage.getItem('paymentHistory');
    this.paymentHistory = data ? JSON.parse(data) : [];
  }

  clearHistory() {
    localStorage.removeItem('paymentHistory');
    this.paymentHistory = [];
  }
}
