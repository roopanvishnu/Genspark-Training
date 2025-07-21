import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { LeaveTypeService } from './leave-type.service';
import { LeaveType } from '../models/leave-type.model';
import { ApiResponse } from '../models/api-response.model';

describe('LeaveTypeService', () => {
  let service: LeaveTypeService;
  let httpMock: HttpTestingController;
  const apiUrl = 'http://localhost:5000/api/v1/leavetypes';

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [LeaveTypeService]
    });
    service = TestBed.inject(LeaveTypeService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should get all leave types', () => {
    const mockTypes: ApiResponse<{ $values: LeaveType[] }> = {
      data: {
        $values: [
          { id: '1', name: 'Sick', standardLeaveCount: 10 },
          { id: '2', name: 'Casual', standardLeaveCount: 8 }
        ]
      }
    };

    service.getLeaveTypes().subscribe(types => {
      expect(types.length).toBe(2);
      expect(types[0].name).toBe('Sick');
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');
    req.flush(mockTypes);
  });

  it('should get leave type by ID', () => {
    const mockType: ApiResponse<LeaveType> = {
      data: { id: '1', name: 'Sick', standardLeaveCount: 10 }
    };

    service.getLeaveTypeById('1').subscribe(res => {
      expect(res.data.name).toBe('Sick');
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(mockType);
  });

  it('should add a new leave type', () => {
    const dto: LeaveType = { id: '3', name: 'Earned', standardLeaveCount: 15 };
    const mockResponse: ApiResponse<LeaveType> = { data: dto };

    service.addLeaveType(dto).subscribe(res => {
      expect(res.data.name).toBe('Earned');
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(dto);
    req.flush(mockResponse);
  });

  it('should update a leave type', () => {
    const dto: LeaveType = { id: '1', name: 'Updated Sick', standardLeaveCount: 12 };
    const mockResponse: ApiResponse<LeaveType> = { data: dto };

    service.updateLeaveType('1', dto).subscribe(res => {
      expect(res.data.name).toBe('Updated Sick');
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(dto);
    req.flush(mockResponse);
  });

  it('should delete a leave type', () => {
    service.deleteLeaveType('1').subscribe(res => {
      expect(res.data).toBeNull();
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('DELETE');
    req.flush({ data: null });
  });
});
