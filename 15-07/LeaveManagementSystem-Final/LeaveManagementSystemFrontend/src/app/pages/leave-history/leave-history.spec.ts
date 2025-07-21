import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { LeaveHistory } from './leave-history';
import { LeaveService } from '../../services/leave.service';
import { of, throwError } from 'rxjs';
import { LeaveRequest } from '../../models/leave-request.model';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterTestingModule } from '@angular/router/testing';

describe('LeaveHistory Component', () => {
  let component: LeaveHistory;
  let fixture: ComponentFixture<LeaveHistory>;
  let mockLeaveService: jasmine.SpyObj<LeaveService>;

  const mockLeaves: LeaveRequest[] = [
    {
      id: '1',
      userId: 'user-001',
      userName: 'Vasu',
      leaveTypeId: 'LT1',
      leaveTypeName: 'Sick Leave',
      startDate: '2025-06-01',
      endDate: '2025-06-02',
      reason: 'Fever and rest',
      status: 'Approved',
      reviewedById: 'hr-001',
      reviewedByName: 'HR Manager',
      createdAt: '2025-05-30',
      updatedAt: '2025-05-31',
    },
    {
      id: '2',
      userId: 'user-001',
      userName: 'Vasu',
      leaveTypeId: 'LT2',
      leaveTypeName: 'Casual Leave',
      startDate: '2025-06-10',
      endDate: '2025-06-12',
      reason: 'Family function',
      status: 'Pending',
      reviewedById: 'hr-002',
      reviewedByName: 'HR Assistant',
      createdAt: '2025-06-01',
      updatedAt: '2025-06-02',
    }
  ];

  beforeEach(async () => {
    mockLeaveService = jasmine.createSpyObj('LeaveService', ['getLeaveHistory']);

    await TestBed.configureTestingModule({
      imports: [LeaveHistory, CommonModule, FormsModule, RouterTestingModule],
      providers: [
        { provide: LeaveService, useValue: mockLeaveService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LeaveHistory);
    component = fixture.componentInstance;

    // Mock userId from localStorage
    spyOn(localStorage, 'getItem').and.returnValue('user-001');
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load leave history on init', fakeAsync(() => {
    mockLeaveService.getLeaveHistory.and.returnValue(
      of({ data: { data: { $values: mockLeaves } } })
    );

    component.ngOnInit();
    tick();

    expect(component.allLeaves.length).toBeGreaterThan(0);
    expect(component.leaveRequests.length).toBeGreaterThan(0);
  }));

  it('should filter leaveRequests based on searchText', fakeAsync(() => {
    mockLeaveService.getLeaveHistory.and.returnValue(
      of({ data: { data: { $values: mockLeaves } } })
    );

    component.ngOnInit();
    tick();

    component.searchText = 'fever';
    component.applyFiltersAndPagination();

    expect(component.leaveRequests.length).toBe(1);
    expect(component.leaveRequests[0].reason.toLowerCase()).toContain('fever');
  }));

  it('should change pages correctly', fakeAsync(() => {
    // Simulate more leaves for pagination
    const extendedLeaves = Array(15).fill(mockLeaves[0]).map((l, i) => ({
      ...l,
      id: `${i + 1}`,
      startDate: `2025-06-${(i + 1).toString().padStart(2, '0')}`,
      reason: `Reason ${i + 1}`
    }));

    mockLeaveService.getLeaveHistory.and.returnValue(
      of({ data: { data: { $values: extendedLeaves } } })
    );

    component.pageSize = 5;
    component.ngOnInit();
    tick();

    expect(component.totalPages).toBe(3);
    expect(component.leaveRequests.length).toBe(5);

    const firstReason = component.leaveRequests[0].reason;

    component.nextPage();
    expect(component.page).toBe(2);
    expect(component.leaveRequests[0].reason).not.toBe(firstReason);
  }));

  it('should show error message on API failure', fakeAsync(() => {
    mockLeaveService.getLeaveHistory.and.returnValue(throwError(() => new Error('API Failed')));
    component.ngOnInit();
    tick();
    expect(component.errorMsg).toContain('Failed to load leave history');
  }));

  it('should load leave history on init', fakeAsync(() => {
    mockLeaveService.getLeaveHistory.and.returnValue(
      of({ data: { data: { $values: mockLeaves } } })
    );

    component.ngOnInit();
    tick();

    expect(component.allLeaves.length).toBeGreaterThan(0);
    expect(component.leaveRequests.length).toBeGreaterThan(0);
  }));

  it('should filter leaveRequests based on searchText', fakeAsync(() => {
    mockLeaveService.getLeaveHistory.and.returnValue(
      of({ data: { data: { $values: mockLeaves } } })
    );

    component.ngOnInit();
    tick();

    component.searchText = 'fever';
    component.applyFiltersAndPagination();

    expect(component.leaveRequests.length).toBe(1);
    expect(component.leaveRequests[0].reason.toLowerCase()).toContain('fever');
  }));

  it('should change pages correctly', fakeAsync(() => {
    // Simulate more leaves for pagination
    const extendedLeaves = Array(15).fill(mockLeaves[0]).map((l, i) => ({
      ...l,
      id: `${i + 1}`,
      startDate: `2025-06-${(i + 1).toString().padStart(2, '0')}`,
      reason: `Reason ${i + 1}`
    }));

    mockLeaveService.getLeaveHistory.and.returnValue(
      of({ data: { data: { $values: extendedLeaves } } })
    );

    component.pageSize = 5;
    component.ngOnInit();
    tick();

    expect(component.totalPages).toBe(3);
    expect(component.leaveRequests.length).toBe(5);

    const firstReason = component.leaveRequests[0].reason;

    component.nextPage();
    expect(component.page).toBe(2);
    expect(component.leaveRequests[0].reason).not.toBe(firstReason);
  }));

  it('should show error message on API failure', fakeAsync(() => {
    mockLeaveService.getLeaveHistory.and.returnValue(throwError(() => new Error('API Failed')));
    component.ngOnInit();
    tick();
    expect(component.errorMsg).toContain('Failed to load leave history');
  }));
});
