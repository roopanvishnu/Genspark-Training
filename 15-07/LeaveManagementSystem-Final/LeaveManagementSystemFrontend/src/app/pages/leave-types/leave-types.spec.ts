import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LeaveTypes } from './leave-types';
import { LeaveTypeService } from '../../services/leave-type.service';
import { ToastrService } from 'ngx-toastr';
import { of, throwError } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

describe('LeaveTypes Component', () => {
  let component: LeaveTypes;
  let fixture: ComponentFixture<LeaveTypes>;
  let leaveTypeServiceSpy: jasmine.SpyObj<LeaveTypeService>;
  let toastrSpy: jasmine.SpyObj<ToastrService>;

  const mockLeaveType = {
    id: '1',
    name: 'Sick Leave',
    standardLeaveCount: 10,
    description: 'Used for illness',
  };

  beforeEach(async () => {
    const leaveTypeServiceMock = jasmine.createSpyObj('LeaveTypeService', [
      'getLeaveTypes',
      'addLeaveType',
      'updateLeaveType',
      'deleteLeaveType',
    ]);

    const toastrServiceMock = jasmine.createSpyObj('ToastrService', [
      'success',
      'error',
      'warning',
    ]);

    await TestBed.configureTestingModule({
      imports: [LeaveTypes, FormsModule, CommonModule],
      providers: [
        { provide: LeaveTypeService, useValue: leaveTypeServiceMock },
        { provide: ToastrService, useValue: toastrServiceMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(LeaveTypes);
    component = fixture.componentInstance;
    leaveTypeServiceSpy = TestBed.inject(LeaveTypeService) as jasmine.SpyObj<LeaveTypeService>;
    toastrSpy = TestBed.inject(ToastrService) as jasmine.SpyObj<ToastrService>;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  
  it('should handle error on loadLeaveTypes()', () => {
    leaveTypeServiceSpy.getLeaveTypes.and.returnValue(throwError(() => new Error('Error')));
    component.loadLeaveTypes();
    expect(toastrSpy.error).toHaveBeenCalledWith('Error fetching leave types', 'Error');
  });


  it('should show validation warning if form is invalid', () => {
    component.selectedLeaveType = { id: '', name: '', standardLeaveCount: 0, description: '' };
    component.isEditing = false;

    component.saveLeaveType();

    expect(toastrSpy.warning).toHaveBeenCalledWith('Name and Max Days/Year must be valid.', 'Validation Error');
    expect(leaveTypeServiceSpy.addLeaveType).not.toHaveBeenCalled();
  });
});
