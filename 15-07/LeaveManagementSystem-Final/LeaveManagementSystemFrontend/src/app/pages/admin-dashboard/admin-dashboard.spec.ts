import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AdminDashboardComponent } from './admin-dashboard';
import { LeaveRequestService } from '../../services/leave-request.service';
import { UserService } from '../../services/user.service';
import { LeaveTypeService } from '../../services/leave-type.service';
import { of } from 'rxjs';

describe('AdminDashboardComponent', () => {
  let component: AdminDashboardComponent;
  let fixture: ComponentFixture<AdminDashboardComponent>;

  const mockLeaveRequestService = {
    getAllLeaveRequests: jasmine.createSpy().and.returnValue(of({
      data: {
        data: {
          $values: [
            {
              startDate: new Date().toISOString(),
              endDate: new Date().toISOString(),
              status: 'Approved'
            },
            {
              startDate: new Date().toISOString(),
              endDate: new Date().toISOString(),
              status: 'Pending'
            },
            {
              startDate: new Date().toISOString(),
              endDate: new Date().toISOString(),
              status: 'Rejected'
            },
            {
              startDate: new Date().toISOString(),
              endDate: new Date().toISOString(),
              status: 'Auto-Rejected'
            }
          ]
        }
      }
    }))
  };

  const mockUserService = {
    getUsers: jasmine.createSpy().and.returnValue(of({
      data: {
        $values: new Array(10).fill({ id: 1, name: 'Test User' })
      }
    }))
  };

  const mockLeaveTypeService = {
    getLeaveTypes: jasmine.createSpy().and.returnValue(of([
      { id: 1, name: 'Sick Leave' },
      { id: 2, name: 'Casual Leave' }
    ]))
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminDashboardComponent],
      providers: [
        { provide: LeaveRequestService, useValue: mockLeaveRequestService },
        { provide: UserService, useValue: mockUserService },
        { provide: LeaveTypeService, useValue: mockLeaveTypeService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AdminDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch leave requests and compute stats', () => {
    component.fetchData();
    expect(mockLeaveRequestService.getAllLeaveRequests).toHaveBeenCalled();
    expect(component.stats.totalLeaves).toBe(4);
    expect(component.stats.approved).toBe(1);
    expect(component.stats.pending).toBe(1);
    expect(component.stats.rejected).toBe(1);
    expect(component.stats.autoRejected).toBe(1);
    expect(component.recentLeaves.length).toBeLessThanOrEqual(5);
    expect(component.leavesToday.length).toBeGreaterThan(0);
  });

  it('should fetch users and update totalUsers stat', () => {
    component.fetchData();
    expect(mockUserService.getUsers).toHaveBeenCalled();
    expect(component.stats.totalUsers).toBe(10);
  });

  it('should fetch leave types and update leaveTypes stat', () => {
    component.fetchData();
    expect(mockLeaveTypeService.getLeaveTypes).toHaveBeenCalled();
    expect(component.stats.leaveTypes).toBe(2);
  });
});
