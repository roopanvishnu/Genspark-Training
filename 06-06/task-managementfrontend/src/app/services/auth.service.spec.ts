import { TestBed } from '@angular/core/testing';
import { AuthService } from './auth.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { Router } from '@angular/router';

fdescribe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(() => {
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        AuthService,
        { provide: Router, useValue: routerSpy }
      ]
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);

    localStorage.clear();
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should store access and refresh tokens on login', () => {
    const mockResponse = {
      accessToken: 'fake-access',
      refreshToken: 'fake-refresh'
    };

    service.login({ email: 'test@example.com', password: '1234' }).subscribe(res => {
      expect(res).toEqual(mockResponse);
      expect(localStorage.getItem('accessToken')).toBe('fake-access');
      expect(localStorage.getItem('refreshToken')).toBe('fake-refresh');
    });

    const req = httpMock.expectOne('https://localhost:7120/api/v1/auth/login');
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
  });

  it('should call register API', () => {
    const user = {
      fullName: 'Test User',
      email: 'test@example.com',
      password: '1234',
      role: 'User'
    };

    service.register(user).subscribe();

    const req = httpMock.expectOne('https://localhost:7120/api/v1/auth/register');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(user);
    req.flush({});
  });

  it('should clear localStorage and navigate on logout', () => {
    localStorage.setItem('accessToken', '123');
    localStorage.setItem('refreshToken', '456');

    service.logout();

    expect(localStorage.getItem('accessToken')).toBeNull();
    expect(localStorage.getItem('refreshToken')).toBeNull();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should return access token', () => {
    localStorage.setItem('accessToken', 'abc123');
    expect(service.getAccessToken()).toBe('abc123');
  });

  it('should return refresh token', () => {
    localStorage.setItem('refreshToken', 'xyz456');
    expect(service.getRefreshToken()).toBe('xyz456');
  });

  it('should call refresh API with refreshToken', () => {
    localStorage.setItem('refreshToken', 'refresh-token-123');

    service.refreshToken().subscribe();

    const req = httpMock.expectOne('https://localhost:7120/api/v1/auth/refresh');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toBe('refresh-token-123');
    req.flush({});
  });

  it('should return true if access token exists', () => {
    localStorage.setItem('accessToken', 'abc');
    expect(service.isLoggedIn()).toBeTrue();
  });

  it('should return false if access token does not exist', () => {
    expect(service.isLoggedIn()).toBeFalse();
  });
});
