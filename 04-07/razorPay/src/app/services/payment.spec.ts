import { TestBed } from '@angular/core/testing';
import { PaymentService } from './payment';
import { Router } from '@angular/router';

fdescribe('PaymentService', () => {
  let service: PaymentService;
  let routerSpy: jasmine.SpyObj<Router>;
  let openSpy: jasmine.Spy;

  beforeEach(() => {
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    openSpy = jasmine.createSpy('open');

    TestBed.configureTestingModule({
      providers: [
        PaymentService,
        { provide: Router, useValue: routerSpy }
      ]
    });

    service = TestBed.inject(PaymentService);

    (window as any).Razorpay = function (options: any) {
      return {
        open: openSpy
      };
    };

    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should save payment result to localStorage', () => {
    const result = {
      status: 'success',
      paymentId: 'pay_123',
      amount: 100,
      name: 'Ram',
      email: 'ram@example.com',
      contact: '9876543210',
      date: new Date().toISOString()
    };

    service.savePaymentResult(result);

    const stored = JSON.parse(localStorage.getItem('paymentHistory') || '[]');
    expect(stored.length).toBe(1);
    expect(stored[0].paymentId).toBe('pay_123');
  });

  it('should call Razorpay and open modal', () => {
    const mockData = {
      amount: 100,
      name: 'Ram',
      email: 'ram@example.com',
      contact: '9876543210'
    };

    service.initiatePayment(mockData);

    // Ensure Razorpay was called and .open was triggered
    expect(typeof (window as any).Razorpay).toBe('function');
    expect(openSpy).toHaveBeenCalled();
  });
});
