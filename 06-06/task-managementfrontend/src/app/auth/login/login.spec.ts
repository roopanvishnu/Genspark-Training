import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Login } from './login';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { of, throwError } from 'rxjs';

fdescribe('Login Component', () => {
  let component: Login;
  let fixture: ComponentFixture<Login>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    const authSpy = jasmine.createSpyObj('AuthService', ['login']);
    const routerSpyObj = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [Login, CommonModule, FormsModule],
      providers: [
        { provide: AuthService, useValue: authSpy },
        { provide: Router, useValue: routerSpyObj }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Login);
    component = fixture.componentInstance;
    authServiceSpy = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize email, password, and error as empty strings', () => {
    expect(component.email).toBe('');
    expect(component.password).toBe('');
    expect(component.error).toBe('');
  });

  it('should call AuthService.login and navigate on success', () => {
    const mockCreds = { email: 'test@example.com', password: 'password' };
    component.email = mockCreds.email;
    component.password = mockCreds.password;

    authServiceSpy.login.and.returnValue(of({ success: true }));

    component.onSubmit();

    expect(authServiceSpy.login).toHaveBeenCalledWith(mockCreds);
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/home']);
    expect(component.error).toBe('');
  });

  it('should set error message on failed login', () => {
    const mockError = {
      error: { message: 'Invalid credentials' }
    };
    authServiceSpy.login.and.returnValue(throwError(() => mockError));

    component.email = 'wrong@example.com';
    component.password = 'wrongpass';

    component.onSubmit();

    expect(authServiceSpy.login).toHaveBeenCalled();
    expect(component.error).toBe('Invalid credentials');
  });

  it('should fallback to generic error if no message is provided', () => {
    const mockError = {
      error: null
    };
    authServiceSpy.login.and.returnValue(throwError(() => mockError));

    component.onSubmit();

    expect(component.error).toBe('Login failed');
  });
});
