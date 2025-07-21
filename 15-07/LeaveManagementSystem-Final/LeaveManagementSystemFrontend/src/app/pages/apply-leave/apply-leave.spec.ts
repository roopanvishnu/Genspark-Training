import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ApplyLeave } from './apply-leave';
import { ReactiveFormsModule } from '@angular/forms';
import { LeaveService } from '../../services/leave.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { of, throwError } from 'rxjs';
import { By } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';

describe('ApplyLeave Component', () => {
  let component: ApplyLeave;
  let fixture: ComponentFixture<ApplyLeave>;
  let leaveServiceSpy: jasmine.SpyObj<LeaveService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let toastrSpy: jasmine.SpyObj<ToastrService>;

  beforeEach(async () => {
    const leaveServiceMock = jasmine.createSpyObj('LeaveService', [
      'getLeaveTypes',
      'applyLeave',
      'uploadAttachment'
    ]);

    leaveServiceMock.getLeaveTypes.and.returnValue(
      of({
        data: {
          $values: [
            { id: '1', name: 'Sick Leave', standardLeaveCount: 5 },
            { id: '2', name: 'Casual Leave', standardLeaveCount: 10 }
          ]
        }
      })
    );

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, CommonModule, ApplyLeave],
      providers: [
        { provide: LeaveService, useValue: leaveServiceMock },
        { provide: Router, useValue: jasmine.createSpyObj('Router', ['navigate']) },
        { provide: ToastrService, useValue: jasmine.createSpyObj('ToastrService', ['success', 'error', 'warning']) }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ApplyLeave);
    component = fixture.componentInstance;
    leaveServiceSpy = TestBed.inject(LeaveService) as jasmine.SpyObj<LeaveService>;
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    toastrSpy = TestBed.inject(ToastrService) as jasmine.SpyObj<ToastrService>;

    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should show warning if form is invalid on submit', () => {
    component.onSubmit();
    expect(toastrSpy.warning).toHaveBeenCalledWith('Please fill all required fields correctly');
  });

  it('should submit leave without attachments', fakeAsync(() => {
    const mockResponse = {
      data: { id: 'leave123' }
    };
    leaveServiceSpy.applyLeave.and.returnValue(of(mockResponse));
    component.filesToUpload = [];

    component.applyLeaveForm.setValue({
      leaveTypeId: '1',
      startDate: '2025-07-02',
      endDate: '2025-07-03',
      reason: 'Vacation',
      files: null
    });

    component.onSubmit();
    tick();

    expect(leaveServiceSpy.applyLeave).toHaveBeenCalled();
    expect(toastrSpy.success).toHaveBeenCalledWith('Leave applied successfully');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/leave-history']);
  }));

  it('should upload attachments after leave request', fakeAsync(() => {
    const mockResponse = {
      data: { id: 'leave456' }
    };
    leaveServiceSpy.applyLeave.and.returnValue(of(mockResponse));
    leaveServiceSpy.uploadAttachment.and.returnValue(of({}));

    const file = new File(['dummy'], 'test.txt', { type: 'text/plain' });
    component.filesToUpload = [file];

    component.applyLeaveForm.setValue({
      leaveTypeId: '2',
      startDate: '2025-07-05',
      endDate: '2025-07-06',
      reason: 'Medical leave',
      files: null
    });

    component.onSubmit();
    tick();

    expect(leaveServiceSpy.uploadAttachment).toHaveBeenCalled();
    expect(toastrSpy.success).toHaveBeenCalledWith('All attachments uploaded successfully');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/leave-history']);
  }));

  it('should show error when leave request fails', fakeAsync(() => {
    leaveServiceSpy.applyLeave.and.returnValue(
      throwError(() => ({ error: { message: 'Apply failed' } }))
    );

    component.applyLeaveForm.setValue({
      leaveTypeId: '2',
      startDate: '2025-07-01',
      endDate: '2025-07-02',
      reason: 'Emergency',
      files: null
    });

    component.onSubmit();
    tick();

    expect(toastrSpy.error).toHaveBeenCalledWith('Apply failed');
  }));
});
