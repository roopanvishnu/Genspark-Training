import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { UserService } from './user.service';
import { UserDto, CreateUserDto, UpdateUserDto } from '../models/user-dto.model';
import { ApiResponse } from '../models/api-response.model';

describe('UserService', () => {
  let service: UserService;
  let httpMock: HttpTestingController;
  const apiUrl = 'http://localhost:5000/api/v1/Users';

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [UserService]
    });

    service = TestBed.inject(UserService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should fetch users with query params', () => {
    const mockResponse: ApiResponse<{ $values: UserDto[] }> = {
      data: {
        $values: [
          {
            id: '1',
            username: 'alice',
            email: 'alice@example.com',
            role: 'HR',
            gender: 'Female',
            isActive: true,
            createdAt: new Date().toISOString()
          }
        ]
      },
      pagination: {
        page: 1,
        pageSize: 5,
        totalPages: 1,
        totalRecords: 1
      }
    };

    service.getUsers(1, 5, '', '', 'CreatedAt', 'asc').subscribe(res => {
      expect(res.data.$values.length).toBe(1);
      expect(res.data.$values[0].username).toBe('alice');
      expect(res.pagination?.totalRecords).toBe(1);
    });

    const req = httpMock.expectOne(
      `${apiUrl}?page=1&pageSize=5&searchTerm=&role=&sortBy=CreatedAt&sortOrder=asc`
    );
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should fetch user by ID', () => {
    const userId = '123';
    const mockResponse: ApiResponse<UserDto> = {
      data: {
        id: userId,
        username: 'bob',
        email: 'bob@example.com',
        role: 'Employee',
        gender: 'Male',
        isActive: true,
        createdAt: new Date().toISOString()
      }
    };

    service.getUserById(userId).subscribe(res => {
      expect(res.data.id).toBe(userId);
    });

    const req = httpMock.expectOne(`${apiUrl}/${userId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should create a new user', () => {
    const newUser: CreateUserDto = {
      username: 'charlie',
      email: 'charlie@example.com',
      password: 'securePass',
      role: 'Employee',
      gender: 'Other',
      isActive: false
    };

    const mockResponse: ApiResponse<UserDto> = {
      data: {
        id: '99',
        username: 'charlie',
        email: 'charlie@example.com',
        role: 'Employee',
        gender: 'Other',
        isActive: false,
        createdAt: new Date().toISOString()
      }
    };

    service.createUser(newUser).subscribe(res => {
      expect(res.data.username).toBe('charlie');
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(newUser);
    req.flush(mockResponse);
  });

  it('should update a user', () => {
    const userId = '22';
    const updateData: UpdateUserDto = {
      username: 'dave',
      email: 'dave@example.com',
      role: 'HR',
      gender: 'Male',
      isActive: true
    };

    const mockResponse: ApiResponse<UserDto> = {
      data: {
        id: userId,
        username: 'dave',
        email: 'dave@example.com',
        role: 'HR',
        gender: 'Male',
        isActive: true,
        createdAt: new Date().toISOString()
      }
    };

    service.updateUser(userId, updateData).subscribe(res => {
      expect(res.data.username).toBe('dave');
    });

    const req = httpMock.expectOne(`${apiUrl}/${userId}`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(updateData);
    req.flush(mockResponse);
  });

  it('should delete a user', () => {
    const userId = '45';
    const mockResponse: ApiResponse<null> = {
      data: null
    };

    service.deleteUser(userId).subscribe(res => {
      expect(res.data).toBeNull();
    });

    const req = httpMock.expectOne(`${apiUrl}/${userId}`);
    expect(req.request.method).toBe('DELETE');
    req.flush(mockResponse);
  });
});
