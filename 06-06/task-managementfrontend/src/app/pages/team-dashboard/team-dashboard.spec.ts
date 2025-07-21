import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TeamDashboard } from './team-dashboard';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TaskService } from '../../services/task.service';
import { of, throwError } from 'rxjs';

fdescribe('TeamDashboard Component', () => {
  let component: TeamDashboard;
  let fixture: ComponentFixture<TeamDashboard>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;

  beforeEach(async () => {
    const taskSpy = jasmine.createSpyObj('TaskService', [
      'getAssignedTasks',
      'updateTaskStatus',
      'downloadAttachment'
    ]);

    await TestBed.configureTestingModule({
      imports: [TeamDashboard, CommonModule, FormsModule],
      providers: [
        { provide: TaskService, useValue: taskSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(TeamDashboard);
    component = fixture.componentInstance;
    taskServiceSpy = TestBed.inject(TaskService) as jasmine.SpyObj<TaskService>;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load assigned tasks and add newStatus & newComment', () => {
    const tasks = {
      data: [
        { id: '1', title: 'Task 1', status: 'Open' },
        { id: '2', title: 'Task 2', status: 'InProgress' }
      ]
    };
    taskServiceSpy.getAssignedTasks.and.returnValue(of(tasks));

    component.ngOnInit();

    expect(taskServiceSpy.getAssignedTasks).toHaveBeenCalled();
    expect(component.assignedTasks.length).toBe(2);
    expect(component.assignedTasks[0].newStatus).toBe('Open');
    expect(component.assignedTasks[1].newComment).toBe('');
  });

  it('should handle error when loading assigned tasks fails', () => {
    spyOn(console, 'error');
    taskServiceSpy.getAssignedTasks.and.returnValue(throwError(() => new Error('fail')));

    component.ngOnInit();

    expect(console.error).toHaveBeenCalledWith('❌ Failed to load assigned tasks', jasmine.anything());
  });

  it('should update task status and reset comment on success', () => {
    spyOn(window, 'alert');

    const task = {
      id: '1',
      newStatus: 'Completed',
      newComment: 'All done',
      status: 'Open'
    };

    taskServiceSpy.updateTaskStatus.and.returnValue(of({}));

    component.updateTask(task);

    expect(taskServiceSpy.updateTaskStatus).toHaveBeenCalledWith('1', {
      status: 'Completed',
      comment: 'All done'
    });
    expect(task.status).toBe('Completed');
    expect(task.newComment).toBe('');
    expect(window.alert).toHaveBeenCalledWith('✅ Task status updated');
  });

  it('should show alert and log error if task update fails', () => {
    spyOn(window, 'alert');
    spyOn(console, 'error');

    const task = {
      id: '1',
      newStatus: 'Done',
      newComment: 'Finished'
    };

    taskServiceSpy.updateTaskStatus.and.returnValue(throwError(() => new Error('update failed')));

    component.updateTask(task);

    expect(console.error).toHaveBeenCalledWith('❌ Failed to update status', jasmine.anything());
    expect(window.alert).toHaveBeenCalledWith('❌ Failed to update status');
  });

  // it('should download attachment and trigger file download', () => {
  //   const blob = new Blob(['file content'], { type: 'text/plain' });
  //   taskServiceSpy.downloadAttachment.and.returnValue(of(blob));

  //   spyOn(document, 'createElement').and.callThrough();
  //   const clickSpy = jasmine.createSpy();
  //   spyOn(document.body, 'appendChild');
  //   spyOn(URL, 'createObjectURL').and.returnValue('blob:url');
  //   spyOn(URL, 'revokeObjectURL');

  //   const fakeLink = {
  //     href: '',
  //     download: '',
  //     click: clickSpy
  //   };

  //   spyOn(document, 'createElement').and.returnValue(fakeLink as any);

  //   component.downloadAttachment('123');

  //   expect(taskServiceSpy.downloadAttachment).toHaveBeenCalledWith('123');
  //   expect(clickSpy).toHaveBeenCalled();
  //   expect(URL.revokeObjectURL).toHaveBeenCalled();
  // });

  it('should alert if downloadAttachment fails', () => {
    spyOn(window, 'alert');
    spyOn(console, 'error');

    taskServiceSpy.downloadAttachment.and.returnValue(throwError(() => new Error('fail')));

    component.downloadAttachment('456');

    expect(console.error).toHaveBeenCalled();
    expect(window.alert).toHaveBeenCalledWith('No attachment found or access denied');
  });
});
