import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  private razorpayKey = 'rzp_test_I1bHELIBcoH8Ph';

  constructor(private router: Router) {}

  initiatePayment(data: {
    amount: number;
    name: string;
    email: string;
    contact: string;
  }): void {
    console.log('Initiating Razorpay Checkout with:', data);

    const options: any = {
      key: this.razorpayKey,
      amount: data.amount * 100,
      currency: 'INR',
      name: 'UPI Payment Simulator',
      description: 'Test UPI Payment',
      prefill: {
        name: data.name,
        email: data.email,
        contact: data.contact
      },
      method: {
        upi: true
      },
      theme: {
        color: '#1976d2'
      },

      handler: (response: any) => {
        console.log('Payment Success:', response);

        const result = {
          status: 'success',
          paymentId: response.razorpay_payment_id,
          amount: data.amount,
          name: data.name,
          email: data.email,
          contact: data.contact,
          date: new Date().toISOString()
        };

        this.savePaymentResult(result);
        this.router.navigate(['/result']);
      },

      modal: {
        ondismiss: () => {
          console.warn('Payment Cancelled or Closed by User');

          const result = {
            status: 'cancelled',
            paymentId: null,
            amount: data.amount,
            name: data.name,
            email: data.email,
            contact: data.contact,
            date: new Date().toISOString()
          };

          this.savePaymentResult(result);
          this.router.navigate(['/result']);
        }
      }
    };

    const razorpay = new (window as any).Razorpay(options);
    razorpay.open();
  }

  savePaymentResult(result: any): void {
    const history = JSON.parse(localStorage.getItem('paymentHistory') || '[]');
    history.unshift(result);
    localStorage.setItem('paymentHistory', JSON.stringify(history));
    console.log('Payment saved to localStorage:', result);
  }
}
