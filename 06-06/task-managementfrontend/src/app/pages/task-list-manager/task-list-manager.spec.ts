import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TaskListManager } from './task-list-manager';
import { Router } from '@angular/router';
import { TaskService } from '../../services/task.service';
import { of, throwError } from 'rxjs';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

fdescribe('TaskListManager Component', () => {
  let component: TaskListManager;
  let fixture: ComponentFixture<TaskListManager>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    const taskSpy = jasmine.createSpyObj('TaskService', [
  'getAllTasks',
  'broadcastTask',
  'getTaskAttachment' // ✅ ADD THIS
]);

    const routerSpyObj = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [TaskListManager, CommonModule, RouterModule],
      providers: [
        { provide: TaskService, useValue: taskSpy },
        { provide: Router, useValue: routerSpyObj }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(TaskListManager);
    component = fixture.componentInstance;
    taskServiceSpy = TestBed.inject(TaskService) as jasmine.SpyObj<TaskService>;
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch tasks on init', () => {
    const mockRes = {
      data: [{ id: '1', title: 'Task 1' }],
      pagination: { totalRecords: 1 }
    };

    taskServiceSpy.getAllTasks.and.returnValue(of(mockRes));

    component.ngOnInit();

    expect(taskServiceSpy.getAllTasks).toHaveBeenCalledWith(1, 10);
    expect(component.tasks.length).toBe(1);
    expect(component.totalPages).toBe(1);
  });

  it('should handle fetchTasks error', () => {
    spyOn(console, 'error');
    taskServiceSpy.getAllTasks.and.returnValue(throwError(() => new Error('Failed')));

    component.fetchTasks(1);

    expect(console.error).toHaveBeenCalledWith('Failed to fetch tasks:', jasmine.anything());
  });

  it('should return correct pagination pages', () => {
    component.totalPages = 3;
    const pages = component.getPages();
    expect(pages).toEqual([1, 2, 3]);
  });

  it('should navigate to editTask()', () => {
    component.editTask('abc');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/manager/tasks/abc/edit']);
  });

  it('should navigate to assignTask()', () => {
    component.assignTask('xyz');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/manager/tasks/xyz/assign']);
  });

  it('should call getTaskAttachment and simulate download', () => {
  const blob = new Blob(['dummy content'], { type: 'application/pdf' });
  const response = {
    body: blob,
    headers: {
      get: (key: string) => 'attachment; filename="sample.pdf"',
    },
  };

  const createObjectURLSpy = spyOn(window.URL, 'createObjectURL').and.returnValue('blob:sample');
  const revokeObjectURLSpy = spyOn(window.URL, 'revokeObjectURL');
  const clickSpy = jasmine.createSpy();
  const createElementSpy = spyOn(document, 'createElement').and.returnValue({ click: clickSpy } as any);

  taskServiceSpy.getTaskAttachment.and.returnValue(of(response as any));

  component.viewAttachments('file123');

  expect(taskServiceSpy.getTaskAttachment).toHaveBeenCalledWith('file123');
  expect(createObjectURLSpy).toHaveBeenCalled();
  expect(clickSpy).toHaveBeenCalled();
  expect(revokeObjectURLSpy).toHaveBeenCalledWith('blob:sample');
});


  it('should broadcast task and show alert on success', () => {
    spyOn(window, 'alert');
    taskServiceSpy.broadcastTask.and.returnValue(of({}));

    component.broadcastTask('task-1');

    expect(taskServiceSpy.broadcastTask).toHaveBeenCalledWith('task-1');
    expect(window.alert).toHaveBeenCalledWith('✅ Task broadcasted!');
  });

  it('should show alert on broadcast failure', () => {
    spyOn(window, 'alert');
    taskServiceSpy.broadcastTask.and.returnValue(throwError(() => new Error('fail')));

    component.broadcastTask('task-2');

    expect(window.alert).toHaveBeenCalledWith('❌ Failed to broadcast task');
  });
});
