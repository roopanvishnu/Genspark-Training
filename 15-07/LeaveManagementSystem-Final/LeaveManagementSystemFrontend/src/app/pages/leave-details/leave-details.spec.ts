import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LeaveDetails } from './leave-details';
import { ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

import { LeaveRequestService } from '../../services/leave-request.service';
import { LeaveAttachmentService } from '../../services/leave-attachment.service';
import { LeaveBalanceService } from '../../services/leave-balance.service';
import { UserService } from '../../services/user.service';

fdescribe('LeaveDetails', () => {
  let component: LeaveDetails;
  let fixture: ComponentFixture<LeaveDetails>;

  let mockLeaveRequestService = {
    getLeaveRequestById: jasmine.createSpy().and.returnValue(of({ data: { userId: 'u1', leaveTypeId: 'lt1', status: 'Pending' } })),
    getAdminOverrideComment: jasmine.createSpy().and.returnValue(of({ comment: 'Override Comment' })),
    updateLeaveStatus: jasmine.createSpy().and.returnValue(of({})),
    overrideLeaveRequestAsAdmin: jasmine.createSpy().and.returnValue(of({}))
  };

  let mockLeaveAttachmentService = {
    getAttachmentsByLeaveRequestId: jasmine.createSpy().and.returnValue(of({ data: { $values: [{ id: 1 }] } }))
  };

  let mockLeaveBalanceService = {
    getLeaveBalanceForType: jasmine.createSpy().and.returnValue(of({ data: { leaveBalance: { balance: 5 } } }))
  };

  let mockUserService = {
    getCurrentUser: jasmine.createSpy().and.returnValue(of({ role: 'Admin' }))
  };

  let mockToastrService = {
    success: jasmine.createSpy(),
    error: jasmine.createSpy(),
    warning: jasmine.createSpy()
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LeaveDetails],
      providers: [
        { provide: LeaveRequestService, useValue: mockLeaveRequestService },
        { provide: LeaveAttachmentService, useValue: mockLeaveAttachmentService },
        { provide: LeaveBalanceService, useValue: mockLeaveBalanceService },
        { provide: UserService, useValue: mockUserService },
        { provide: ToastrService, useValue: mockToastrService },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: { paramMap: { get: () => 'test-leave-id' } }
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LeaveDetails);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch leave balance for type', () => {
    expect(mockLeaveBalanceService.getLeaveBalanceForType).toHaveBeenCalledWith('u1', 'lt1');
    expect(component.leaveBalanceForType).toBeTruthy();
  });

  it('should update status as HR', () => {
    component.isHr = true;
    component.leaveRequest = { status: 'Pending' } as any;
    component.leaveId = 'test-leave-id';

    component.updateStatus('Approved');

    expect(mockLeaveRequestService.updateLeaveStatus).toHaveBeenCalledWith('test-leave-id', { status: 'Approved' });
  });

  it('should override as Admin', () => {
    component.isAdmin = true;
    component.leaveRequest = { status: 'Approved' } as any;
    component.leaveId = 'test-leave-id';
    component.newStatus = 'Rejected';
    component.overrideComment = 'Policy Violation';

    component.adminOverride();

    expect(mockLeaveRequestService.overrideLeaveRequestAsAdmin).toHaveBeenCalled();
  });

  it('should handle errors in loadDetails', () => {
    mockLeaveRequestService.getLeaveRequestById.and.returnValue(throwError(() => new Error('Fetch error')));
    component.loadDetails();
    expect(mockToastrService.error).toHaveBeenCalledWith('Failed to load leave details', 'Error');
  });

  it('should return correct status class', () => {
    expect(component.getStatusClass('Pending')).toBe('status-pending');
    expect(component.getStatusClass('Approved')).toBe('status-approved');
    expect(component.getStatusClass('Rejected')).toBe('status-rejected');
  });
});
