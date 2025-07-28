import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { LeaveBalance } from './leave-balance';
import { LeaveBalanceService } from '../../services/leave-balance.service';
import { UserService } from '../../services/user.service';
import { LeaveTypeService } from '../../services/leave-type.service';
import { AuthStateService } from '../../services/auth-state.service';
import { ToastrService } from 'ngx-toastr';
import { of, throwError } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

describe('LeaveBalance Component', () => {
  let component: LeaveBalance;
  let fixture: ComponentFixture<LeaveBalance>;

  let leaveBalanceServiceSpy: jasmine.SpyObj<LeaveBalanceService>;
  let userServiceSpy: jasmine.SpyObj<UserService>;
  let leaveTypeServiceSpy: jasmine.SpyObj<LeaveTypeService>;
  let toastrServiceSpy: jasmine.SpyObj<ToastrService>;

  beforeEach(async () => {
    leaveBalanceServiceSpy = jasmine.createSpyObj('LeaveBalanceService', [
      'getLeaveBalance',
      'initializeLeaveBalance',
      'initializeLeaveType',
      'resetLeaveBalance',
      'deductLeave'
    ]);

    userServiceSpy = jasmine.createSpyObj('UserService', ['getUsers']);
    leaveTypeServiceSpy = jasmine.createSpyObj('LeaveTypeService', ['getLeaveTypes']);
    toastrServiceSpy = jasmine.createSpyObj('ToastrService', ['error', 'success']);

    await TestBed.configureTestingModule({
      imports: [FormsModule, CommonModule],
      declarations: [LeaveBalance],
      providers: [
        { provide: LeaveBalanceService, useValue: leaveBalanceServiceSpy },
        { provide: UserService, useValue: userServiceSpy },
        { provide: LeaveTypeService, useValue: leaveTypeServiceSpy },
        { provide: ToastrService, useValue: toastrServiceSpy },
        { provide: AuthStateService, useValue: {} }
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LeaveBalance);
    component = fixture.componentInstance;
    localStorage.setItem('userId', '123');
    localStorage.setItem('role', 'Admin');
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('should load balances, leave types and users if Admin or HR', () => {
      spyOn(component, 'loadSelfBalance');
      spyOn(component, 'loadLeaveTypes');
      spyOn(component, 'loadAllUsers');

      component.ngOnInit();

      expect(component.loadSelfBalance).toHaveBeenCalled();
      expect(component.loadLeaveTypes).toHaveBeenCalled();
      expect(component.loadAllUsers).toHaveBeenCalled();
    });
  });

  it('should load self balance on loadSelfBalance()', () => {
    const mockData = {
      data: {
        leaveBalances: {
          $values: [{ type: 'Sick Leave', count: 5 }]
        }
      }
    };
    leaveBalanceServiceSpy.getLeaveBalance.and.returnValue;

    component.loadSelfBalance();

    expect(leaveBalanceServiceSpy.getLeaveBalance).toHaveBeenCalledWith('123');
    expect(component.balances.length).toBe(1);
  });

  it('should handle error on loadSelfBalance()', () => {
    leaveBalanceServiceSpy.getLeaveBalance.and.returnValue(throwError(() => new Error()));
    component.loadSelfBalance();
    expect(toastrServiceSpy.error).toHaveBeenCalledWith('Failed to load leave balance.', 'Error');
  });

  it('should view another user balance', () => {
    component.selectedUserIdForView = '456';
    const mockData = { data: { leaveBalances: { $values: [{ type: 'Annual', count: 10 }] } } };
    leaveBalanceServiceSpy.getLeaveBalance.and.returnValue

    component.viewOtherUserBalance();
    expect(component.selectedUserBalance.length).toBe(1);
  });

  it('should call initializeLeaveBalance()', () => {
    component.selectedUserIdForInit = '789';
    leaveBalanceServiceSpy.initializeLeaveBalance.and.returnValue;

    component.initializeBalance();
    expect(leaveBalanceServiceSpy.initializeLeaveBalance).toHaveBeenCalledWith('789');
    expect(toastrServiceSpy.success).toHaveBeenCalled();
  });

  it('should call initializeLeaveType()', () => {
    component.selectedUserIdForTypeInit = '789';
    component.selectedLeaveTypeIdForTypeInit = '1';
    component.standardLeaveCountForTypeInit = 5;
    leaveBalanceServiceSpy.initializeLeaveType.and.returnValue;

    component.initializeTypeBalance();
    expect(leaveBalanceServiceSpy.initializeLeaveType).toHaveBeenCalledWith('789', '1', 5);
    expect(toastrServiceSpy.success).toHaveBeenCalled();
  });

  it('should reset leave balances', () => {
    component.selectedUserIdForReset = '999';
    leaveBalanceServiceSpy.resetLeaveBalance.and.returnValue;

    component.resetBalance();
    expect(leaveBalanceServiceSpy.resetLeaveBalance).toHaveBeenCalledWith('999');
    expect(toastrServiceSpy.success).toHaveBeenCalled();
  });

  it('should deduct leave', () => {
    component.selectedUserIdForDeduct = '321';
    component.deductLeaveTypeId = '2';
    component.deductDays = 3;
    leaveBalanceServiceSpy.deductLeave.and.returnValue;

    component.deductLeave();
    expect(leaveBalanceServiceSpy.deductLeave).toHaveBeenCalledWith('321', '2', 3);
    expect(toastrServiceSpy.success).toHaveBeenCalled();
  });

  it('should handle error on deductLeave', () => {
    component.selectedUserIdForDeduct = '321';
    component.deductLeaveTypeId = '2';
    component.deductDays = 3;

    leaveBalanceServiceSpy.deductLeave.and.returnValue(
      throwError(() => ({
        error: { message: 'Leave deduction failed' }
      }))
    );

    component.deductLeave();
    expect(toastrServiceSpy.error).toHaveBeenCalledWith('Leave deduction failed', 'Error');
  });
});
