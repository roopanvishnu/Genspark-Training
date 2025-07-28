import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ApplyLeave } from './apply-leave';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { of, throwError } from 'rxjs';

import { LeaveTypeService } from '../../services/leave-type.service';
import { LeaveRequestService } from '../../services/leave-request.service';
import { LeaveAttachmentService } from '../../services/leave-attachment.service';

describe('ApplyLeave', () => {
  let component: ApplyLeave;
  let fixture: ComponentFixture<ApplyLeave>;

  const mockRouter = {
    navigate: jasmine.createSpy('navigate')
  };

  const mockToastr = {
    success: jasmine.createSpy('success'),
    error: jasmine.createSpy('error'),
    warning: jasmine.createSpy('warning')
  };

  const mockLeaveTypeService = {
    getLeaveTypes: jasmine.createSpy().and.returnValue(of([
      { id: '1', name: 'Sick Leave' },
      { id: '2', name: 'Casual Leave' }
    ]))
  };

  const mockLeaveRequestService = {
    createLeaveRequest: jasmine.createSpy().and.returnValue(of({
      data: { id: 'mock-leave-id' }
    }))
  };

  const mockLeaveAttachmentService = {
    uploadAttachment: jasmine.createSpy().and.returnValue(of({}))
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ApplyLeave, ReactiveFormsModule],
      providers: [
        FormBuilder,
        { provide: Router, useValue: mockRouter },
        { provide: ToastrService, useValue: mockToastr },
        { provide: LeaveTypeService, useValue: mockLeaveTypeService },
        { provide: LeaveRequestService, useValue: mockLeaveRequestService },
        { provide: LeaveAttachmentService, useValue: mockLeaveAttachmentService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ApplyLeave);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize form with required controls', () => {
    expect(component.applyLeaveForm).toBeTruthy();
    expect(component.applyLeaveForm.contains('leaveTypeId')).toBeTrue();
    expect(component.applyLeaveForm.contains('startDate')).toBeTrue();
    expect(component.applyLeaveForm.contains('endDate')).toBeTrue();
    expect(component.applyLeaveForm.contains('reason')).toBeTrue();
    expect(component.applyLeaveForm.contains('files')).toBeTrue();
  });

  it('should load leave types on init', () => {
    expect(mockLeaveTypeService.getLeaveTypes).toHaveBeenCalled();
    expect(component.leaveTypes.length).toBeGreaterThan(0);
  });

  it('should warn if form is invalid on submit', () => {
    component.applyLeaveForm.patchValue({
      leaveTypeId: '',
      startDate: '',
      endDate: '',
      reason: ''
    });

    component.onSubmit();

    expect(mockToastr.warning).toHaveBeenCalledWith('Please fill all required fields correctly');
    expect(mockLeaveRequestService.createLeaveRequest).not.toHaveBeenCalled();
  });

  it('should call leave request service and navigate if no files', () => {
    component.applyLeaveForm.setValue({
      leaveTypeId: '1',
      startDate: '2025-08-01',
      endDate: '2025-08-03',
      reason: 'Medical leave',
      files: null
    });

    component.filesToUpload = [];

    component.onSubmit();

    expect(mockLeaveRequestService.createLeaveRequest).toHaveBeenCalled();
    expect(mockToastr.success).toHaveBeenCalledWith('Leave applied successfully');
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/leave-history']);
  });

  it('should upload files if provided and show success', () => {
    component.applyLeaveForm.setValue({
      leaveTypeId: '1',
      startDate: '2025-08-01',
      endDate: '2025-08-03',
      reason: 'Medical leave',
      files: null
    });

    const fakeFile = new File(['dummy content'], 'file1.pdf');
    component.filesToUpload = [fakeFile];

    component.onSubmit();

    expect(mockLeaveAttachmentService.uploadAttachment).toHaveBeenCalledWith('mock-leave-id', fakeFile);
  });

  it('should show error message if leave request fails', () => {
    mockLeaveRequestService.createLeaveRequest.and.returnValue(throwError(() => ({
      error: { message: 'Server error' }
    })));

    component.applyLeaveForm.setValue({
      leaveTypeId: '1',
      startDate: '2025-08-01',
      endDate: '2025-08-03',
      reason: 'Vacation',
      files: null
    });

    component.onSubmit();

    expect(mockToastr.error).toHaveBeenCalledWith('Server error');
  });

  it('should handle file input', () => {
    const mockEvent = {
      target: {
        files: [new File(['content'], 'test.jpg')]
      }
    } as unknown as Event;

    component.handleFileInput(mockEvent);

    expect(component.filesToUpload.length).toBe(1);
    expect(component.filesToUpload[0].name).toBe('test.jpg');
  });

  it('should set error if endDate < startDate', () => {
    const start = new Date('2025-08-10');
    const end = new Date('2025-08-05');

    component.applyLeaveForm.patchValue({
      startDate: start,
      endDate: end
    });

    component.ngOnInit(); // triggers the validator again

    component.applyLeaveForm.get('startDate')?.setValue(start);
    component.applyLeaveForm.get('endDate')?.setValue(end);

    expect(component.applyLeaveForm.get('endDate')?.errors?.['dateMismatch']).toBeTrue();
  });
});
