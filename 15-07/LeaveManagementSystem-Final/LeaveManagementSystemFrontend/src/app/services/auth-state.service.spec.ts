import { TestBed } from '@angular/core/testing';
import { AuthStateService } from './auth-state.service';


describe('AuthStateService', () => {
  let service: AuthStateService;

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      providers: [AuthStateService]
    });
    service = TestBed.inject(AuthStateService);
  });

  afterEach(() => {
    localStorage.clear();
  });

  it('should initialize isLoggedIn$ as false when no token is in localStorage', (done) => {
    service.isLoggedIn$.subscribe(value => {
      expect(value).toBeFalse();
      done();
    });
  });

  it('should set auth state and update observables', (done) => {
    service.setAuthState(true, 'HR', 'chellz');

    service.isLoggedIn$.subscribe(loggedIn => {
      expect(loggedIn).toBeTrue();
    });
    service.role$.subscribe(role => {
      expect(role).toBe('HR');
    });
    service.username$.subscribe(username => {
      expect(username).toBe('chellz');
      done();
    });
  });

  it('should clear state and localStorage on logout', (done) => {
    localStorage.setItem('accessToken', 'token123');
    localStorage.setItem('role', 'HR');
    localStorage.setItem('username', 'chellz');

    service.logout();

    service.isLoggedIn$.subscribe(loggedIn => {
      expect(loggedIn).toBeFalse();
    });
    service.role$.subscribe(role => {
      expect(role).toBeNull();
    });
    service.username$.subscribe(username => {
      expect(username).toBeNull();
      done();
    });
  });
});
