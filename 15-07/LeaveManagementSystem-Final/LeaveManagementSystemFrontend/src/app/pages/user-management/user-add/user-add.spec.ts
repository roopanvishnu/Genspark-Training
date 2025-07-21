import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { UserAdd } from './user-add';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../../../services/user.service';
import { ToastrService } from 'ngx-toastr';
import { of, throwError } from 'rxjs';
import { CreateUserDto, UserDto } from '../../../models/user-dto.model';

describe('UserAdd Component', () => {
  let component: UserAdd;
  let fixture: ComponentFixture<UserAdd>;
  let userServiceSpy: jasmine.SpyObj<UserService>;
  let toastrServiceSpy: jasmine.SpyObj<ToastrService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    const userSpy = jasmine.createSpyObj('UserService', ['createUser']);
    const toastrSpy = jasmine.createSpyObj('ToastrService', ['success', 'error']);
    const routeSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [UserAdd, FormsModule],
      providers: [
        { provide: UserService, useValue: userSpy },
        { provide: ToastrService, useValue: toastrSpy },
        { provide: Router, useValue: routeSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(UserAdd);
    component = fixture.componentInstance;
    userServiceSpy = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
    toastrServiceSpy = TestBed.inject(ToastrService) as jasmine.SpyObj<ToastrService>;
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should call createUser and navigate on success', fakeAsync(() => {
    const mockUser: UserDto = {
      id: 'u1',
      username: 'testuser',
      email: 'test@example.com',
      role: 'Employee',
      gender: 'Male',
      isActive: true,
      createdAt: new Date().toISOString()
    };

    userServiceSpy.createUser.and.returnValue(of({ data: mockUser }));

    component.userForm = {
      username: 'testuser',
      email: 'test@example.com',
      password: '123456',
      role: 'Employee',
      gender: 'Male',
      isActive: true
    };

    const mockForm: any = { invalid: false };
    component.saveUser(mockForm);
    tick();

    expect(userServiceSpy.createUser).toHaveBeenCalled();
    expect(toastrServiceSpy.success).toHaveBeenCalledWith('User created successfully!', 'Success');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/users']);
    expect(component.isLoading).toBeFalse();
  }));

  it('should show error on failed createUser', fakeAsync(() => {
    const mockError = { error: { message: 'Server error' } };
    userServiceSpy.createUser.and.returnValue(throwError(() => mockError));

    component.userForm = {
      username: 'testuser',
      email: 'test@example.com',
      password: '123456',
      role: 'Employee',
      gender: 'Male',
      isActive: true
    };

    const mockForm: any = { invalid: false };
    component.saveUser(mockForm);
    tick();

    expect(userServiceSpy.createUser).toHaveBeenCalled();
    expect(toastrServiceSpy.error).toHaveBeenCalledWith('Server error', 'Error');
    expect(component.isLoading).toBeFalse();
  }));

  it('should not call createUser if form is invalid', () => {
    const mockForm: any = { invalid: true };
    component.saveUser(mockForm);
    expect(userServiceSpy.createUser).not.toHaveBeenCalled();
  });
});
