import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { LeaveDetails } from './leave-details';
import { LeaveService } from '../../services/leave.service';
import { LeaveBalanceService } from '../../services/leave-balance.service';
import { ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';

describe('LeaveDetails Component', () => {
  let component: LeaveDetails;
  let fixture: ComponentFixture<LeaveDetails>;

  const leaveServiceSpy = jasmine.createSpyObj('LeaveService', [
    'getLeaveRequestById',
    'getLeaveAttachmentsByRequestId',
    'updateLeaveStatus'
  ]);
  const balanceServiceSpy = jasmine.createSpyObj('LeaveBalanceService', ['getLeaveBalanceForType']);
  const toastrSpy = jasmine.createSpyObj('ToastrService', ['success', 'error', 'warning']);

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LeaveDetails, CommonModule],
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

    fixture = TestBed.createComponent(LeaveDetails);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load leave details successfully', fakeAsync(() => {
    leaveServiceSpy.getLeaveRequestById.and.returnValue(of({
      data: {
        userId: 'u1',
        leaveTypeId: 'lt1',
        userName: 'User',
        leaveTypeName: 'Sick Leave',
        status: 'Pending',
        startDate: new Date(),
        endDate: new Date(),
        reason: 'Test Reason'
      }
    }));
    leaveServiceSpy.getLeaveAttachmentsByRequestId.and.returnValue(of({
      data: {
        $values: [
          { fileName: 'proof.pdf', downloadUrl: 'url', uploadedAt: new Date() }
        ]
      }
    }));
    balanceServiceSpy.getLeaveBalanceForType.and.returnValue(of({
      data: {
        leaveBalance: {
          totalLeaves: 10,
          usedLeaves: 3,
          remainingLeaves: 7
        }
      }
    }));

    fixture.detectChanges(); // triggers ngOnInit
    tick();

    expect(component.leaveRequest.userName).toBe('User');
    expect(component.attachments.length).toBe(1);
    expect(component.leaveBalanceForType.remainingLeaves).toBe(7);
  }));

  it('should handle error if leave request fetch fails', fakeAsync(() => {
    leaveServiceSpy.getLeaveRequestById.and.returnValue(throwError(() => new Error('Error loading')));
    fixture.detectChanges();
    tick();

    expect(component.errorMsg).toBe('Failed to load leave details');
    expect(toastrSpy.error).toHaveBeenCalledWith('Failed to load leave details', 'Error');
  }));

  it('should update leave status successfully', fakeAsync(() => {
    component.leaveId = 'mockLeaveId';
    leaveServiceSpy.updateLeaveStatus.and.returnValue(of({}));
    leaveServiceSpy.getLeaveRequestById.and.returnValue(of({
      data: { userId: 'u1', leaveTypeId: 'lt1' }
    }));

    component.updateStatus('Approved');
    tick();

    expect(toastrSpy.success).toHaveBeenCalledWith('Leave request approved successfully!', 'Success');
  }));

  it('should handle update status failure', fakeAsync(() => {
    component.leaveId = 'mockLeaveId';
    leaveServiceSpy.updateLeaveStatus.and.returnValue(throwError(() => ({
      error: { message: 'Update failed' }
    })));

    component.updateStatus('Rejected');
    tick();

    expect(toastrSpy.error).toHaveBeenCalledWith('Failed to update status. Update failed', 'Error');
  }));
});
