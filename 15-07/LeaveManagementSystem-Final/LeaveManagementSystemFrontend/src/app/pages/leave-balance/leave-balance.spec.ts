import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { LeaveBalance } from './leave-balance';
import { LeaveBalanceService } from '../../services/leave-balance.service';
import { UserService } from '../../services/user.service';
import { LeaveService } from '../../services/leave.service';
import { AuthStateService } from '../../services/auth-state.service';
import { ToastrService } from 'ngx-toastr';
import { of, throwError } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { ApiResponse } from '../../models/api-response.model';
import { UserLeaveBalanceResponseDto } from '../../models/user-leave-balance.model';
import { UserDto } from '../../models/user-dto.model';

describe('LeaveBalance Component', () => {
  let component: LeaveBalance;
  let fixture: ComponentFixture<LeaveBalance>;
  let balanceServiceSpy: jasmine.SpyObj<LeaveBalanceService>;
  let userServiceSpy: jasmine.SpyObj<UserService>;
  let leaveServiceSpy: jasmine.SpyObj<LeaveService>;
  let toastrSpy: jasmine.SpyObj<ToastrService>;

  beforeEach(async () => {
    const balanceMock = jasmine.createSpyObj('LeaveBalanceService', [
      'getLeaveBalance', 'initializeLeaveBalance', 'initializeLeaveType', 'resetLeaveBalance', 'deductLeave'
    ]);
    const userMock = jasmine.createSpyObj('UserService', ['getUsers']);
    const leaveTypeMock = jasmine.createSpyObj('LeaveService', ['getLeaveTypes']);

    await TestBed.configureTestingModule({
      imports: [LeaveBalance, FormsModule, CommonModule],
      providers: [
        { provide: LeaveBalanceService, useValue: balanceMock },
        { provide: UserService, useValue: userMock },
        { provide: LeaveService, useValue: leaveTypeMock },
        { provide: ToastrService, useValue: jasmine.createSpyObj('ToastrService', ['success', 'error']) },
        { provide: AuthStateService, useValue: {} }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LeaveBalance);
    component = fixture.componentInstance;
    balanceServiceSpy = TestBed.inject(LeaveBalanceService) as jasmine.SpyObj<LeaveBalanceService>;
    userServiceSpy = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
    leaveServiceSpy = TestBed.inject(LeaveService) as jasmine.SpyObj<LeaveService>;
    toastrSpy = TestBed.inject(ToastrService) as jasmine.SpyObj<ToastrService>;

    // Updated: Set default mocks
    leaveServiceSpy.getLeaveTypes.and.returnValue(of([
      { id: 'lt1', name: 'Sick Leave' },
      { id: 'lt2', name: 'Casual Leave' }
    ]));

    spyOn(localStorage, 'getItem').and.callFake((key: string) => {
      const mockStorage: { [key: string]: string } = {
        userId: 'user123',
        role: 'HR'
      };
      return mockStorage[key] ?? null;
    });
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  
});
