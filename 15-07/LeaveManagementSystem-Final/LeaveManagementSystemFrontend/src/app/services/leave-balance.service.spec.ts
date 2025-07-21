import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { LeaveBalanceService } from './leave-balance.service';
import { ApiResponse } from '../models/api-response.model';
import { UserLeaveBalanceResponseDto } from '../models/user-leave-balance.model';


describe('LeaveBalanceService', () => {
  let service: LeaveBalanceService;
  let httpMock: HttpTestingController;
  const baseUrl = 'http://localhost:5000/api/v1';

  beforeEach(() => {
    localStorage.setItem('accessToken', 'mock-token');
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [LeaveBalanceService]
    });
    service = TestBed.inject(LeaveBalanceService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    localStorage.clear();
    httpMock.verify();
  });

  it('should get full leave balance', () => {
    const mockResponse: ApiResponse<UserLeaveBalanceResponseDto> = {
      data: {
        userId: 'u1',
        userName: 'Chellz',
        leaveBalances: [
          { leaveTypeId: 'lt1', leaveTypeName: 'Sick Leave', remainingDays: 8, totalDays: 10 }
        ]
      }
    };

    service.getLeaveBalance('u1').subscribe(res => {
      expect(res.data.userName).toBe('Chellz');
      expect(res.data.leaveBalances.length).toBe(1);
    });

    const req = httpMock.expectOne(`${baseUrl}/leave-balance/user/u1`);
    expect(req.request.method).toBe('GET');
    expect(req.request.headers.get('Authorization')).toBe('Bearer mock-token');
    req.flush(mockResponse);
  });

  it('should get balance for a specific leave type', () => {
    const mockTypeBalance = {
      leaveTypeId: 'lt1', leaveTypeName: 'Sick Leave', totalLeaves: 10, usedLeaves: 2, remainingLeaves: 8
    };

    service.getLeaveBalanceForType('u1', 'lt1').subscribe(res => {
      expect(res.remainingLeaves).toBe(8);
    });

    const req = httpMock.expectOne(`${baseUrl}/leave-balance/user/u1/leaveType/lt1`);
    expect(req.request.method).toBe('GET');
    req.flush(mockTypeBalance);
  });

  it('should initialize user leave balance', () => {
    service.initializeLeaveBalance('u1').subscribe(res => {
      expect(res.data).toBe('Initialized');
    });

    const req = httpMock.expectOne(`${baseUrl}/leave-balance/initialize/user/u1`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({});
    req.flush({ data: 'Initialized' });
  });

  it('should initialize leave type for user', () => {
    service.initializeLeaveType('u1', 'lt1', 15).subscribe(res => {
      expect(res.data).toBe('Leave Type Initialized');
    });

    const req = httpMock.expectOne(`${baseUrl}/leave-balance/initialize/user/u1/leaveType/lt1?standardLeaveCount=15`);
    expect(req.request.method).toBe('POST');
    req.flush({ data: 'Leave Type Initialized' });
  });

  it('should deduct leave for a user', () => {
    service.deductLeave('u1', 'lt1', 2).subscribe(res => {
      expect(res.data).toBe('Deducted');
    });

    const req = httpMock.expectOne(`${baseUrl}/leave-balance/deduct?userId=u1&leaveTypeId=lt1&days=2`);
    expect(req.request.method).toBe('POST');
    req.flush({ data: 'Deducted' });
  });

  it('should reset leave balance', () => {
    service.resetLeaveBalance('u1').subscribe(res => {
      expect(res.data).toBe('Reset');
    });

    const req = httpMock.expectOne(`${baseUrl}/leave-balance/reset/user/u1`);
    expect(req.request.method).toBe('POST');
    req.flush({ data: 'Reset' });
  });
});
