import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Home } from './home';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from '../../services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError } from 'rxjs';

fdescribe('Home Component', () => {
  let component: Home;
  let fixture: ComponentFixture<Home>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
  const authSpy = jasmine.createSpyObj('AuthService', ['logout']);
  const routerSpyObj = jasmine.createSpyObj('Router', ['navigate']);

  await TestBed.configureTestingModule({
    imports: [HttpClientTestingModule, Home],
    providers: [
      { provide: AuthService, useValue: authSpy },
      { provide: Router, useValue: routerSpyObj },
      {
        provide: ActivatedRoute,
        useValue: {
          snapshot: { params: {}, queryParams: {} },
          paramMap: of(new Map()),
        }
      }
    ]
  }).compileComponents();

  fixture = TestBed.createComponent(Home);
  component = fixture.componentInstance;
  authServiceSpy = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
  routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  httpMock = TestBed.inject(HttpTestingController);

  localStorage.clear();
});
  afterEach(() => {
    httpMock.verify(); // verify no outstanding requests
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should decode token and set user on valid token', () => {
  const fakePayload = {
    'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name': 'John Doe',
    'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress': 'john@example.com',
    'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': 'Manager'
  };
  const token = 'header.' + btoa(JSON.stringify(fakePayload)) + '.signature';
  localStorage.setItem('accessToken', token);

  component.ngOnInit();

  expect(component.user?.fullName).toBe('John Doe');
  expect(component.user?.email).toBe('john@example.com');
  expect(component.user?.role).toBe('Manager');

  const req = httpMock.expectOne('https://localhost:7120/api/v1/users?page=1&pageSize=10');
  req.flush({ data: [], pagination: { totalPages: 1 } });
});


  it('should call fetchUserFromApi if token is missing', () => {
    spyOn(component, 'fetchUserFromApi');
    localStorage.removeItem('accessToken');

    component.ngOnInit();

    expect(component.fetchUserFromApi).not.toHaveBeenCalled(); // because token is absent, not malformed
  });
  it('should call fetchUserFromApi if fullName and email are missing in token payload', () => {
  const partialPayload = {
    'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': 'Manager'
  };

  const token = 'header.' + btoa(JSON.stringify(partialPayload)) + '.signature';
  localStorage.setItem('accessToken', token);

  spyOn(component, 'fetchUserFromApi');

  component.ngOnInit();

  expect(component.fetchUserFromApi).toHaveBeenCalled();

  const req = httpMock.expectOne('https://localhost:7120/api/v1/users?page=1&pageSize=10');
  req.flush({ data: [], pagination: { totalPages: 1 } });
});


  it('should call fetchUserFromApi if token is invalid JSON', () => {
  spyOn(component, 'fetchUserFromApi');

  localStorage.setItem('accessToken', 'header.invalidbase64.signature');

  component.ngOnInit();

  expect(component.fetchUserFromApi).toHaveBeenCalled();
});


  it('getInitials should return correct initials', () => {
    expect(component.getInitials('John Doe')).toBe('JD');
    expect(component.getInitials('SingleName')).toBe('S');
    expect(component.getInitials('')).toBe('?');
    expect(component.getInitials(undefined)).toBe('?');
    
  });

  it('logout should clear and navigate to /login', () => {
    component.logout();
    expect(authServiceSpy.logout).toHaveBeenCalled();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('fetchUserFromApi should set user on success', () => {
    component.fetchUserFromApi();

    const req = httpMock.expectOne('https://localhost:7120/api/v1/auth/me');
    expect(req.request.method).toBe('GET');

    req.flush({ data: { fullName: 'API User', email: 'api@example.com', role: 'Admin' } });

    expect(component.user?.fullName).toBe('API User');
  });

  it('fetchUserFromApi should log error on failure', () => {
    spyOn(console, 'error');

    component.fetchUserFromApi();

    const req = httpMock.expectOne('https://localhost:7120/api/v1/auth/me');
    req.error(new ErrorEvent('Network error'));

    expect(console.error).toHaveBeenCalledWith(
      'Failed to fetch user from /auth/me:',
      jasmine.anything()
    );
  });
});
