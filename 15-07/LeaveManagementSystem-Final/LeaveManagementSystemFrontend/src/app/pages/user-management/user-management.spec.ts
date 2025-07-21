import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UserManagement } from './user-management';
import { UserService } from '../../services/user.service';
import { of, throwError } from 'rxjs';
import { Router, NavigationEnd } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { UserDto } from '../../models/user-dto.model';

describe('UserManagement Component', () => {
  let component: UserManagement;
  let fixture: ComponentFixture<UserManagement>;
  let userServiceSpy: jasmine.SpyObj<UserService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let toastrSpy: jasmine.SpyObj<ToastrService>;

  const mockUsers: UserDto[] = [
    {
      id: '1',
      username: 'JohnDoe',
      email: 'john@example.com',
      role: 'Employee',
      gender: 'Male',
      isActive: true,
      createdAt: '2024-01-01T00:00:00Z'
    },
    {
      id: '2',
      username: 'JaneSmith',
      email: 'jane@example.com',
      role: 'HR',
      gender: 'Female',
      isActive: false,
      createdAt: '2024-01-01T00:00:00Z'
    }
  ];

  beforeEach(async () => {
    userServiceSpy = jasmine.createSpyObj<UserService>('UserService', ['getUsers', 'deleteUser']);
    routerSpy = jasmine.createSpyObj<Router>('Router', ['navigate'], { events: of(new NavigationEnd(1, '', '')) });
    toastrSpy = jasmine.createSpyObj<ToastrService>('ToastrService', ['success', 'error']);

    await TestBed.configureTestingModule({
      imports: [UserManagement],
      providers: [
        { provide: UserService, useValue: userServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: ToastrService, useValue: toastrSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(UserManagement);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load users on init', () => {
    userServiceSpy.getUsers.and.returnValue(of({
      data: { $values: mockUsers },
      pagination: {
        totalPages: 1,
        totalRecords: 2,
        page: 1,
        pageSize: 10
      }
    }));

    fixture.detectChanges();
    expect(userServiceSpy.getUsers).toHaveBeenCalled();
    expect(component.users.length).toBe(2);
    expect(component.totalPages).toBe(1);
  });

  it('should apply filters and reload users', () => {
    userServiceSpy.getUsers.and.returnValue(of({
      data: { $values: mockUsers },
      pagination: {
        totalPages: 1,
        totalRecords: 2,
        page: 1,
        pageSize: 10
      }
    }));

    component.searchTerm = 'john';
    component.applyFilters();

    expect(component.page).toBe(1);
    expect(userServiceSpy.getUsers).toHaveBeenCalled();
  });

  it('should handle delete user success', () => {
    userServiceSpy.deleteUser.and.returnValue(of({
      data: null,
      pagination: {
        totalPages: 0,
        totalRecords: 0,
        page: 0,
        pageSize: 0
      }
    }));

    userServiceSpy.getUsers.and.returnValue(of({
      data: { $values: mockUsers },
      pagination: {
        totalPages: 1,
        totalRecords: 2,
        page: 1,
        pageSize: 10
      }
    }));

    spyOn(window, 'confirm').and.returnValue(true);

    component.deleteUser('1');

    expect(userServiceSpy.deleteUser).toHaveBeenCalledWith('1');
    expect(toastrSpy.success).toHaveBeenCalled();
    expect(userServiceSpy.getUsers).toHaveBeenCalled();
  });

  it('should handle delete user error', () => {
    userServiceSpy.deleteUser.and.returnValue(throwError(() => ({
      error: { message: 'Delete failed' }
    })));

    spyOn(window, 'confirm').and.returnValue(true);

    component.deleteUser('invalid-id');

    expect(userServiceSpy.deleteUser).toHaveBeenCalledWith('invalid-id');
    expect(toastrSpy.error).toHaveBeenCalledWith('Delete failed', 'Error');
  });

  it('should navigate to add user', () => {
    component.navigateToAddUser();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/users/add']);
  });

  it('should navigate to edit user', () => {
    component.navigateToEditUser('123');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/users/edit', '123']);
  });
});
