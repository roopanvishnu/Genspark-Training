import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { Login } from './login';
import { ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { AuthStateService } from '../../services/auth-state.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { of, throwError } from 'rxjs';
import { By } from '@angular/platform-browser';

describe('Login Component', () => {
  let component: Login;
  let fixture: ComponentFixture<Login>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let authStateSpy: jasmine.SpyObj<AuthStateService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let toastrSpy: jasmine.SpyObj<ToastrService>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, Login],
      providers: [
        { provide: AuthService, useValue: jasmine.createSpyObj('AuthService', ['login']) },
        { provide: AuthStateService, useValue: jasmine.createSpyObj('AuthStateService', ['setAuthState']) },
        { provide: Router, useValue: jasmine.createSpyObj('Router', ['navigate']) },
        { provide: ToastrService, useValue: jasmine.createSpyObj('ToastrService', ['success', 'error']) }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Login);
    component = fixture.componentInstance;
    authServiceSpy = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    authStateSpy = TestBed.inject(AuthStateService) as jasmine.SpyObj<AuthStateService>;
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    toastrSpy = TestBed.inject(ToastrService) as jasmine.SpyObj<ToastrService>;

    fixture.detectChanges();
  });

  it('should create the login component', () => {
    expect(component).toBeTruthy();
  });

  it('should show validation messages when form is invalid and submitted', () => {
    component.loginForm.markAllAsTouched();
    fixture.detectChanges();

    const errorMsgs = fixture.debugElement.queryAll(By.css('.text-red-500'));
    expect(errorMsgs.length).toBeGreaterThan(0);
  });

  it('should call AuthService.login on form submit success', fakeAsync(() => {
    const mockResponse = {
      accessToken: 'token',
      refreshToken: 'refresh',
      user: {
        id: '1', username: 'user', email: 'user@example.com', role: 'HR'
      }
    };
    authServiceSpy.login.and.returnValue(of(mockResponse));

    component.loginForm.setValue({ email: 'user@example.com', password: '123456' });
    component.onSubmit();
    tick();

    expect(authServiceSpy.login).toHaveBeenCalledWith('user@example.com', '123456');
    expect(authStateSpy.setAuthState).toHaveBeenCalledWith(true, 'HR', 'user');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/dashboard']);
    expect(toastrSpy.success).toHaveBeenCalled();
  }));

   it('should show validation messages when form is invalid', () => {
    component.loginForm.markAllAsTouched();
    fixture.detectChanges();

    const errorMsgs = fixture.debugElement.queryAll(By.css('.text-red-500'));
    expect(errorMsgs.length).toBeGreaterThan(0);
  });

  it('should show error message on login failure', fakeAsync(() => {
    authServiceSpy.login.and.returnValue(throwError(() => ({ error: { message: 'Invalid credentials' } })));
    component.loginForm.setValue({ email: 'wrong@example.com', password: 'badpass' });

    component.onSubmit();
    tick();

    expect(component.errorMsg).toBe('Invalid credentials');
    expect(toastrSpy.error).toHaveBeenCalledWith('Invalid credentials', 'Login Failed');
  }));
});