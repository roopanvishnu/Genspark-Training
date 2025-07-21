import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Register } from './register';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { of, throwError } from 'rxjs';

fdescribe('Register Component', () => {
  let component: Register;
  let fixture: ComponentFixture<Register>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    const authSpy = jasmine.createSpyObj('AuthService', ['register']);
    const routerSpyObj = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [Register, CommonModule, FormsModule],
      providers: [
        { provide: AuthService, useValue: authSpy },
        { provide: Router, useValue: routerSpyObj }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Register);
    component = fixture.componentInstance;
    authServiceSpy = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize all form fields as empty', () => {
    expect(component.fullName).toBe('');
    expect(component.email).toBe('');
    expect(component.password).toBe('');
    expect(component.confirmPassword).toBe('');
    expect(component.role).toBe('');
    expect(component.error).toBe('');
  });

  it('should show error if passwords do not match', () => {
    component.password = '123';
    component.confirmPassword = '456';

    component.onSubmit();

    expect(component.error).toBe('Passwords do not match.');
    expect(authServiceSpy.register).not.toHaveBeenCalled();
  });

  it('should call AuthService.register and navigate on success', () => {
    component.fullName = 'John Doe';
    component.email = 'john@example.com';
    component.password = '123456';
    component.confirmPassword = '123456';
    component.role = 'User';

    authServiceSpy.register.and.returnValue(of({ success: true }));

    component.onSubmit();

    expect(authServiceSpy.register).toHaveBeenCalledWith({
      fullName: 'John Doe',
      email: 'john@example.com',
      password: '123456',
      role: 'User'
    });
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
    expect(component.error).toBe('');
  });

  it('should set error message on registration failure', () => {
    const errorResponse = {
      error: { message: 'Email already exists' }
    };

    authServiceSpy.register.and.returnValue(throwError(() => errorResponse));

    component.fullName = 'Jane';
    component.email = 'jane@example.com';
    component.password = 'abc';
    component.confirmPassword = 'abc';
    component.role = 'Admin';

    component.onSubmit();

    expect(component.error).toBe('Email already exists');
  });

  it('should fallback to generic error message if error message is missing', () => {
    authServiceSpy.register.and.returnValue(throwError(() => ({ error: null })));

    component.password = '123';
    component.confirmPassword = '123';

    component.onSubmit();

    expect(component.error).toBe('Registration failed');
  });
});
