import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { LeaveHistoryById } from './leave-history-by-id';
import { LeaveService } from '../../services/leave.service';
import { LeaveBalanceService } from '../../services/leave-balance.service';
import { ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';

describe('LeaveHistoryById Component', () => {
  let component: LeaveHistoryById;
  let fixture: ComponentFixture<LeaveHistoryById>;

  const leaveServiceSpy = jasmine.createSpyObj('LeaveService', [
    'getLeaveRequestById',
    'getLeaveAttachmentsByRequestId',
    'updateLeaveStatus',
    'uploadAttachment',
    'deleteAttachment',
    'cancelLeaveRequest'
  ]);

  const balanceServiceSpy = jasmine.createSpyObj('LeaveBalanceService', ['getLeaveBalanceForType']);
  const toastrSpy = jasmine.createSpyObj('ToastrService', ['success', 'error']);

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LeaveHistoryById, CommonModule],
      providers: [
        { provide: LeaveService, useValue: leaveServiceSpy },
        { provide: LeaveBalanceService, useValue: balanceServiceSpy },
        { provide: ToastrService, useValue: toastrSpy },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: {
                get: () => 'mockLeaveId'
              }
            }
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LeaveHistoryById);
    component = fixture.componentInstance;
  });

  it('should create component', () => {
    expect(component).toBeTruthy();
  });

  it('should load leave details, attachments, and balance', fakeAsync(() => {
    leaveServiceSpy.getLeaveRequestById.and.returnValue(of({
      data: {
        userId: 'u1',
        leaveTypeId: 'lt1',
        userName: 'TestUser',
        leaveTypeName: 'CL',
        status: 'Pending',
        startDate: new Date(),
        endDate: new Date(),
        reason: 'Reason'
      }
    }));

    leaveServiceSpy.getLeaveAttachmentsByRequestId.and.returnValue(of({
      data: { $values: [{ id: 'a1', fileName: 'doc.pdf', downloadUrl: 'url', uploadedAt: new Date() }] }
    }));

    balanceServiceSpy.getLeaveBalanceForType.and.returnValue(of({
      data: {
        leaveBalance: {
          totalLeaves: 12,
          usedLeaves: 4,
          remainingLeaves: 8
        }
      }
    }));

    fixture.detectChanges(); // triggers ngOnInit
    tick();

    expect(component.leaveRequest.userName).toBe('TestUser');
    expect(component.attachments.length).toBe(1);
    expect(component.leaveBalanceForType.remainingLeaves).toBe(8);
  }));

  it('should handle leave request fetch failure', fakeAsync(() => {
    leaveServiceSpy.getLeaveRequestById.and.returnValue(throwError(() => new Error('Fail')));
    fixture.detectChanges();
    tick();
    expect(component.errorMsg).toBe('Failed to load leave details');
  }));

  it('should delete attachment and update list', fakeAsync(() => {
    spyOn(window, 'confirm').and.returnValue(true);
    component.attachments = [{ id: 'a1', fileName: 'file1.pdf' }];
    leaveServiceSpy.deleteAttachment.and.returnValue(of({}));

    component.deleteAttachment('a1');
    tick();

    expect(component.attachments.length).toBe(0);
  }));

  it('should upload file and refresh attachments', fakeAsync(() => {
    const mockFile = new File(['data'], 'test.pdf');
    component.selectedFile = mockFile;
    component.leaveId = 'mockLeaveId';

    leaveServiceSpy.uploadAttachment.and.returnValue(of({}));
    leaveServiceSpy.getLeaveAttachmentsByRequestId.and.returnValue(of({
      data: { $values: [{ id: 'a2', fileName: 'test.pdf' }] }
    }));

    component.uploadAttachment();
    tick();

    expect(component.attachments.length).toBe(1);
    expect(component.attachments[0].fileName).toBe('test.pdf');
    expect(component.selectedFile).toBeNull();
  }));

  it('should update leave status and refetch details', fakeAsync(() => {
    component.leaveId = 'mockLeaveId';

    leaveServiceSpy.updateLeaveStatus.and.returnValue(of({}));
    leaveServiceSpy.getLeaveRequestById.and.returnValue(of({
      data: {
        userId: 'u1',
        leaveTypeId: 'lt1',
        userName: 'UpdatedUser',
        leaveTypeName: 'CL',
        status: 'Approved'
      }
    }));

    spyOn(window, 'alert');

    component.updateStatus('Approved');
    tick();

    expect(leaveServiceSpy.updateLeaveStatus).toHaveBeenCalledWith('mockLeaveId', 'Approved');
    expect(component.leaveRequest.userName).toBe('UpdatedUser');
    expect(window.alert).toHaveBeenCalledWith('Leave request has been approved successfully!');
  }));

  it('should cancel leave request and show success toast', fakeAsync(() => {
    spyOn(window, 'confirm').and.returnValue(true);
    component.leaveId = 'mockLeaveId';

    leaveServiceSpy.cancelLeaveRequest.and.returnValue(of({}));
    leaveServiceSpy.getLeaveRequestById.and.returnValue(of({
      data: {
        userId: 'u1',
        leaveTypeId: 'lt1',
        userName: 'User',
        status: 'Cancelled'
      }
    }));

    component.cancelLeaveRequest();
    tick();

    expect(toastrSpy.success).toHaveBeenCalledWith('Leave request cancelled successfully.', 'Cancelled');
    expect(component.leaveRequest.status).toBe('Cancelled');
  }));

  it('should show error toast if cancellation fails', fakeAsync(() => {
    spyOn(window, 'confirm').and.returnValue(true);
    component.leaveId = 'mockLeaveId';

    leaveServiceSpy.cancelLeaveRequest.and.returnValue(throwError(() => ({
      error: { message: 'Server error' }
    })));

    component.cancelLeaveRequest();
    tick();

    expect(toastrSpy.error).toHaveBeenCalledWith('Server error', 'Error');
  }));
});
