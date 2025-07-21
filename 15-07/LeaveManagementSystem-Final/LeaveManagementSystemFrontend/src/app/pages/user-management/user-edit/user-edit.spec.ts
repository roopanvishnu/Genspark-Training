import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { UserEdit } from './user-edit';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../../../services/user.service';
import { ToastrService } from 'ngx-toastr';
import { of, throwError } from 'rxjs';
import { UpdateUserDto, UserDto } from '../../../models/user-dto.model';

describe('UserEdit Component', () => {
  let component: UserEdit;
  let fixture: ComponentFixture<UserEdit>;
  let userServiceSpy: jasmine.SpyObj<UserService>;
  let toastrServiceSpy: jasmine.SpyObj<ToastrService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let routeSpy: any;

  beforeEach(async () => {
    const userSpy = jasmine.createSpyObj('UserService', ['getUserById', 'updateUser']);
    const toastrSpy = jasmine.createSpyObj('ToastrService', ['success', 'error']);
    const routerMock = jasmine.createSpyObj('Router', ['navigate']);
    routeSpy = {
      snapshot: {
        paramMap: {
          get: () => '123' // mock user ID
        }
      }
    };

    await TestBed.configureTestingModule({
      imports: [UserEdit, FormsModule],
      providers: [
        { provide: UserService, useValue: userSpy },
        { provide: ToastrService, useValue: toastrSpy },
        { provide: Router, useValue: routerMock },
        { provide: ActivatedRoute, useValue: routeSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(UserEdit);
    component = fixture.componentInstance;
    userServiceSpy = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
    toastrServiceSpy = TestBed.inject(ToastrService) as jasmine.SpyObj<ToastrService>;
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch user data on init', () => {
    const mockUser: UserDto = {
      id: '123',
      username: 'john',
      email: 'john@example.com',
      role: 'HR',
      gender: 'Male',
      isActive: true,
      createdAt: new Date().toISOString()
    };

    userServiceSpy.getUserById.and.returnValue(of({ data: mockUser }));

    component.ngOnInit();

    expect(component.userForm.username).toBe('john');
    expect(userServiceSpy.getUserById).toHaveBeenCalledWith('123');
  });

  it('should show error if getUserById fails', () => {
    userServiceSpy.getUserById.and.returnValue(throwError(() => ({ error: { message: 'Failed to load' } })));

    component.ngOnInit();

    expect(toastrServiceSpy.error).toHaveBeenCalledWith('Failed to load user data', 'Error');
  });


  it('should show error on failed updateUser', fakeAsync(() => {
    userServiceSpy.updateUser.and.returnValue(throwError(() => ({ error: { message: 'Update failed' } })));

    component.userId = '123';
    component.userForm = {
      username: 'failUser',
      email: 'fail@example.com',
      role: 'HR',
      gender: 'Other',
      isActive: false
    };

    const formMock: any = { invalid: false };
    component.saveUser(formMock);
    tick();

    expect(toastrServiceSpy.error).toHaveBeenCalledWith('Update failed', 'Update Failed');
    expect(component.isLoading).toBeFalse();
  }));

  it('should not update if form is invalid', () => {
    const formMock: any = { invalid: true };
    component.saveUser(formMock);
    expect(userServiceSpy.updateUser).not.toHaveBeenCalled();
  });
});
