import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PaymentResult } from './payment-result';

fdescribe('PaymentResult', () => {
  let component: PaymentResult;
  let fixture: ComponentFixture<PaymentResult>;

  const mockHistory = [
    {
      status: 'success',
      paymentId: 'pay_123',
      amount: 100,
      name: 'Ram',
      email: 'ram@example.com',
      contact: '9876543210',
      date: new Date().toISOString()
    }
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PaymentResult]
    }).compileComponents();

    localStorage.setItem('paymentHistory', JSON.stringify(mockHistory));

    fixture = TestBed.createComponent(PaymentResult);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  afterEach(() => {
    localStorage.clear();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load payment history from localStorage on init', () => {
    expect(component.paymentHistory.length).toBe(1);
    expect(component.paymentHistory[0].paymentId).toBe('pay_123');
  });

  it('should clear payment history', () => {
    component.clearHistory();
    expect(component.paymentHistory.length).toBe(0);
    expect(localStorage.getItem('paymentHistory')).toBeNull();
  });
});
