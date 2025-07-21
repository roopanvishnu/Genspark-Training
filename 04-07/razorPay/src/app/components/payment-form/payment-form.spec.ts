import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PaymentForm } from './payment-form';
import { ReactiveFormsModule } from '@angular/forms';
import { PaymentService } from '../../services/payment';
import { of } from 'rxjs';

fdescribe('PaymentForm', () => {
  let component: PaymentForm;
  let fixture: ComponentFixture<PaymentForm>;
  let paymentServiceSpy: jasmine.SpyObj<PaymentService>;

  beforeEach(async () => {
    paymentServiceSpy = jasmine.createSpyObj('PaymentService', ['initiatePayment']);

    await TestBed.configureTestingModule({
      imports: [PaymentForm, ReactiveFormsModule],
      providers: [
        { provide: PaymentService, useValue: paymentServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PaymentForm);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should have an invalid form initially', () => {
    expect(component.paymentForm.valid).toBeFalse();
  });

  it('should make the form valid with correct values', () => {
    component.paymentForm.setValue({
      amount: 100,
      name: 'Test User',
      email: 'test@example.com',
      contact: '9876543210'
    });

    expect(component.paymentForm.valid).toBeTrue();
  });

  it('should call initiatePayment when form is valid and submitted', () => {
    component.paymentForm.setValue({
      amount: 100,
      name: 'Test User',
      email: 'test@example.com',
      contact: '9876543210'
    });

    component.onSubmit();

    expect(paymentServiceSpy.initiatePayment).toHaveBeenCalledWith({
      amount: 100,
      name: 'Test User',
      email: 'test@example.com',
      contact: '9876543210'
    });
  });
});
