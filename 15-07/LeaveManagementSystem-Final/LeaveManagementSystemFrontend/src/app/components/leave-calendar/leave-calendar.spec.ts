import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { LeaveCalendarComponent } from './leave-calendar';
import { LeaveService } from '../../services/leave.service';
import { AuthStateService } from '../../services/auth-state.service';
import { of } from 'rxjs';
import { CalendarStandaloneModule } from '../../calendar-standalone.module';

describe('LeaveCalendarComponent', () => {
  let component: LeaveCalendarComponent;
  let fixture: ComponentFixture<LeaveCalendarComponent>;
  let leaveServiceSpy: jasmine.SpyObj<LeaveService>;
  let authStateServiceSpy: jasmine.SpyObj<AuthStateService>;

  beforeEach(async () => {
    // Create spy objects for services
    leaveServiceSpy = jasmine.createSpyObj('LeaveService', ['getLeaveTypes', 'getAllLeaveRequests']);
    authStateServiceSpy = jasmine.createSpyObj('AuthStateService', ['getRole']);

    await TestBed.configureTestingModule({
      imports: [LeaveCalendarComponent, CalendarStandaloneModule], 
      providers: [
        { provide: LeaveService, useValue: leaveServiceSpy },
        { provide: AuthStateService, useValue: authStateServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LeaveCalendarComponent);
    component = fixture.componentInstance;
  });

  it('should create and load leave types and events', fakeAsync(() => {
    authStateServiceSpy.getRole.and.returnValue('HR');

    leaveServiceSpy.getLeaveTypes.and.returnValue(of({
      data: { $values: [{ id: '1', name: 'Casual Leave' }] }
    }));

    leaveServiceSpy.getAllLeaveRequests.and.returnValue(of({
      data: { $values: [{
        userName: 'Alice',
        leaveTypeId: '1',
        startDate: '2024-01-01',
        endDate: '2024-01-03',
        status: 'Approved'
      }] }
    }));

    fixture.detectChanges(); // triggers ngOnInit
    tick(); // wait for observables

    expect(component.isHR).toBeTrue();
    expect(leaveServiceSpy.getLeaveTypes).toHaveBeenCalled();
    expect(leaveServiceSpy.getAllLeaveRequests).toHaveBeenCalled();
    expect(component.leaveTypeMap['1']).toBe('Casual Leave');
    expect(component.events.length).toBe(1);
    expect(component.events[0].title).toContain('Alice');
  }));

  it('should set isHR to false when role is not HR', fakeAsync(() => {
    authStateServiceSpy.getRole.and.returnValue('Employee');
    leaveServiceSpy.getLeaveTypes.and.returnValue(of({ data: { $values: [] } }));
    leaveServiceSpy.getAllLeaveRequests.and.returnValue(of({ data: { $values: [] } }));

    fixture.detectChanges();
    tick();

    expect(component.isHR).toBeFalse();
  }));

  it('should map status to correct color', () => {
    expect(component.getColor('Approved')).toBe('#22c55e');
    expect(component.getColor('Pending')).toBe('#eab308');
    expect(component.getColor('Rejected')).toBe('#ef4444');
    expect(component.getColor('Auto-Rejected')).toBe('#6b7280');
    expect(component.getColor('Cancelled')).toBe('#9ca3af');
    expect(component.getColor('Unknown')).toBe('#a1a1aa');
  });
});
