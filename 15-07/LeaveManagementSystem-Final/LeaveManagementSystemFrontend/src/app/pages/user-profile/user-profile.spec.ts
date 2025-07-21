import { TestBed, ComponentFixture } from '@angular/core/testing';
import { HttpClient } from '@angular/common/http';
import { of, throwError } from 'rxjs';
import { UserProfile } from './user-profile';
import { UserService } from '../../services/user.service';
import { ToastrService } from 'ngx-toastr';

describe('UserProfile Component', () => {
  let fixture: ComponentFixture<UserProfile>;
  let component: UserProfile;

  let httpClientSpy: jasmine.SpyObj<HttpClient>;
  let userServiceSpy: jasmine.SpyObj<UserService>;
  let toastrSpy: jasmine.SpyObj<ToastrService>;

  const mockUser = {
    id: '123',
    username: 'testuser',
    email: 'test@example.com',
    gender: 'Female',
    role: 'Employee',
    isActive: true,
    createdAt: new Date().toISOString()
  };

  beforeEach(async () => {
    httpClientSpy = jasmine.createSpyObj('HttpClient', ['get', 'post']);
    userServiceSpy = jasmine.createSpyObj('UserService', ['updateUser']);
    toastrSpy = jasmine.createSpyObj('ToastrService', ['success', 'error']);

    await TestBed.configureTestingModule({
      imports: [UserProfile], // ✅ import standalone component
      providers: [
        { provide: HttpClient, useValue: httpClientSpy },
        { provide: UserService, useValue: userServiceSpy },
        { provide: ToastrService, useValue: toastrSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(UserProfile);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch user profile on init', () => {
    httpClientSpy.get.and.returnValue(of(mockUser));
    spyOn(localStorage, 'getItem').and.returnValue('fake-token');

    component.ngOnInit();

    expect(component.isLoading).toBeFalse();
    expect(component.user).toEqual(mockUser);
    expect(httpClientSpy.get).toHaveBeenCalled();
  });

  it('should show error if profile fetch fails', () => {
    httpClientSpy.get.and.returnValue(throwError(() => new Error('Failed')));
    spyOn(localStorage, 'getItem').and.returnValue('fake-token');

    component.fetchUserProfile();

    expect(component.errorMsg).toBe('Failed to load profile.');
    expect(component.isLoading).toBeFalse();
  });

  it('should open and close the edit modal', () => {
    component.user = mockUser;
    component.openEditModal();

    expect(component.isModalOpen).toBeTrue();
    expect(component.updatedUser.username).toBe(mockUser.username);

    component.closeModal();
    expect(component.isModalOpen).toBeFalse();
  });

  it('should open and close password modal', () => {
    component.user = mockUser;

    component.openChangePwdModal();
    expect(component.isPwdModalOpen).toBeTrue();
    expect(component.changePwdDto.email).toBe(mockUser.email);

    component.closePwdModal();
    expect(component.isPwdModalOpen).toBeFalse();
  });

  it('should submit password change if valid', () => {
    const mockResponse = of({});
    httpClientSpy.post.and.returnValue(mockResponse);

    component.user = mockUser;
    component.openChangePwdModal();

    component.changePwdDto.currentPassword = 'old123';
    component.changePwdDto.newPassword = 'new123';
    component.changePwdDto.confirmNewPassword = 'new123';

    spyOn(window, 'alert');
    component.submitChangePassword();

    expect(httpClientSpy.post).toHaveBeenCalled();
    expect(window.alert).toHaveBeenCalledWith('Password changed successfully!');
  });

  it('should validate password confirmation mismatch', () => {
    component.changePwdDto.newPassword = 'new123';
    component.changePwdDto.confirmNewPassword = 'wrong';

    component.submitChangePassword();

    expect(component.pwdError).toBe('New password and confirm password do not match.');
  });
});
