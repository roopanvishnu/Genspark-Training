// notifications.spec.ts
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Notifications } from './notifications';
import { CommonModule } from '@angular/common';
import { SignalRService } from '../../services/signal-r.service';

fdescribe('Notifications Component', () => {
  let component: Notifications;
  let fixture: ComponentFixture<Notifications>;

  const handlers: Record<string, (data: any) => void> = {};
  const fakeConnection = {
    on: (event: string, cb: any) => (handlers[event] = cb),
    start: jasmine.createSpy().and.returnValue(Promise.resolve())
  };

  const mockSignalRService = {
    createConnection: () => fakeConnection
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Notifications, CommonModule],
      providers: [
        { provide: SignalRService, useValue: mockSignalRService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Notifications);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should connect and register handlers', () => {
    component.ngOnInit();

    expect(fakeConnection.start).toHaveBeenCalled();
    expect(Object.keys(handlers)).toContain('taskCreated');
    expect(Object.keys(handlers)).toContain('taskUpdated');
    expect(Object.keys(handlers)).toContain('taskAssigned');
    expect(Object.keys(handlers)).toContain('tasksBroadcast');
  });
  

  it('should handle taskCreated event', () => {
    component.ngOnInit();
    handlers['taskCreated']({ title: 'A', taskId: '1' });

    expect(component.messages[0]).toEqual({
      message: 'Task Created: A (ID: 1)',
      type: 'success'
    });
  });

  it('should handle taskUpdated event', () => {
    component.ngOnInit();
    handlers['taskUpdated']({ taskId: '2', status: 'Done' });

    expect(component.messages[0]).toEqual({
      message: 'Task Updated: ID 2, Status: Done',
      type: 'info'
    });
  });

  it('should handle taskAssigned event', () => {
    component.ngOnInit();
    handlers['taskAssigned']({ taskId: '3', assignee: 'Bob' });

    expect(component.messages[0]).toEqual({
      message: 'Task Assigned to Bob (ID: 3)',
      type: 'warning'
    });
  });

  it('should handle tasksBroadcast event', () => {
    component.ngOnInit();
    handlers['tasksBroadcast']({ assignedCount: 4, title: 'Urgent Task' });

    expect(component.messages[0]).toEqual({
      message: '4 users received broadcasted task: Urgent Task',
      type: 'error'
    });
  });
});
