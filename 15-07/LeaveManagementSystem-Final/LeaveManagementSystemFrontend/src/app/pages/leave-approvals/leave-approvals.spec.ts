import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { LeaveApprovals } from './leave-approvals';
import { LeaveService } from '../../services/leave.service';
import { of, throwError } from 'rxjs';
import { RouterTestingModule } from '@angular/router/testing';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

describe('LeaveApprovals Component', () => {
  let component: LeaveApprovals;
  let fixture: ComponentFixture<LeaveApprovals>;
  let leaveServiceSpy: jasmine.SpyObj<LeaveService>;

  const mockLeaveData = {
    data: {
      data: {
        $values: [
          {
            id: '1',
            userName: 'Alice',
            leaveTypeName: 'Sick Leave',
            startDate: '2025-07-01',
            endDate: '2025-07-02',
            reason: 'Fever',
            status: 'Pending',
            createdAt: '2025-06-30T12:00:00Z'
          },
          {
            id: '2',
            userName: 'Bob',
            leaveTypeName: 'Casual Leave',
            startDate: '2025-07-03',
            endDate: '2025-07-05',
            reason: 'Travel',
            status: 'Approved',
            createdAt: '2025-06-29T12:00:00Z'
          }
        ]
      }
    }
  };

  beforeEach(async () => {
    const leaveServiceMock = jasmine.createSpyObj('LeaveService', ['getAllLeaveRequests']);
    leaveServiceMock.getAllLeaveRequests.and.returnValue(of(mockLeaveData));

    await TestBed.configureTestingModule({
      imports: [LeaveApprovals, FormsModule, CommonModule, RouterTestingModule],
      providers: [
        { provide: LeaveService, useValue: leaveServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LeaveApprovals);
    component = fixture.componentInstance;
    leaveServiceSpy = TestBed.inject(LeaveService) as jasmine.SpyObj<LeaveService>;

    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load and store leave requests on init', () => {
    expect(leaveServiceSpy.getAllLeaveRequests).toHaveBeenCalled();
    expect(component.leaveRequests.length).toBe(2);
    expect(component.filteredRequests.length).toBeGreaterThan(0);
  });

  it('should filter requests by search term', () => {
    component.searchTerm = 'bob';
    component.filterRequests();
    expect(component.filteredRequests.length).toBe(1);
    expect(component.filteredRequests[0].userName).toBe('Bob');
  });

  it('should filter requests by status', () => {
    component.statusFilter = 'Approved';
    component.filterRequests();
    expect(component.filteredRequests.length).toBe(1);
    expect(component.filteredRequests[0].status).toBe('Approved');
  });

  it('should paginate correctly', () => {
    component.pageSize = 1;
    component.filterRequests();
    expect(component.totalPages).toBe(2);

    const firstPageUser = component.filteredRequests[0].userName;
    component.nextPage();
    const secondPageUser = component.filteredRequests[0].userName;

    expect(firstPageUser).not.toBe(secondPageUser);
  });

  it('should handle fetch error gracefully', fakeAsync(() => {
    leaveServiceSpy.getAllLeaveRequests.and.returnValue(
      throwError(() => new Error('API error'))
    );

    component.fetchData();
    tick();
    expect(component.errorMsg).toBe('Error loading leave approvals');
  }));

  it('should load and store leave requests on init', () => {
    expect(leaveServiceSpy.getAllLeaveRequests).toHaveBeenCalled();
    expect(component.leaveRequests.length).toBe(2);
    expect(component.filteredRequests.length).toBeGreaterThan(0);
  });

  it('should filter requests by search term and store in filtered request', () => {
    component.searchTerm = 'bob';
    component.filterRequests();
    expect(component.filteredRequests.length).toBe(1);
    expect(component.filteredRequests[0].userName).toBe('Bob');
  });

  it('should filter requests by status - approved, rejected, pending', () => {
    component.statusFilter = 'Approved';
    component.filterRequests();
    expect(component.filteredRequests.length).toBe(1);
    expect(component.filteredRequests[0].status).toBe('Approved');
  });

  it('should paginate correctly as per user changes', () => {
    component.pageSize = 1;
    component.filterRequests();
    expect(component.totalPages).toBe(2);

    const firstPageUser = component.filteredRequests[0].userName;
    component.nextPage();
    const secondPageUser = component.filteredRequests[0].userName;

    expect(firstPageUser).not.toBe(secondPageUser);
  });

  it('should handle fetch error gracefully', fakeAsync(() => {
    leaveServiceSpy.getAllLeaveRequests.and.returnValue(
      throwError(() => new Error('API error'))
    );

    component.fetchData();
    tick();
    expect(component.errorMsg).toBe('Error loading leave approvals');
  }));
});
