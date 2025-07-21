import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from './auth.service';
import { LoginResponse } from '../models/login-response';


describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  const baseUrl = 'http://localhost:5000/api/v1/auth';

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthService]
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should login and return access & refresh tokens with user', () => {
    const mockLoginResponse: LoginResponse = {
      accessToken: 'mockAccessToken',
      refreshToken: 'mockRefreshToken',
      user: {
        id: 'u1',
        username: 'chella',
        email: 'chellz@example.com',
        role: 'HR'
      }
    };

    service.login('chellz@example.com', 'secret').subscribe(res => {
      expect(res.accessToken).toBe('mockAccessToken');
      expect(res.user.username).toBe('chella');
    });

    const req = httpMock.expectOne(`${baseUrl}/login`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({
      email: 'chellz@example.com',
      password: 'secret'
    });

    req.flush(mockLoginResponse);
  });
});
