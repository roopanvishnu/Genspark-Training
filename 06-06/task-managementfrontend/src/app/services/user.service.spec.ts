import { TestBed } from '@angular/core/testing';
import { UserService } from './user.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

fdescribe('UserService', () => {
  let service: UserService;
  let httpMock: HttpTestingController;

  const baseUrl = 'https://localhost:7120/api/v1/users';

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

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch team members with correct query params', () => {
    service.getTeamMembers().subscribe();

    const req = httpMock.expectOne(`${baseUrl}?page=1&pageSize=100`);
    expect(req.request.method).toBe('GET');

    req.flush({ data: [] });
  });
});
