import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TaskListDeleted } from './task-list-deleted';
import { TaskService } from '../../services/task.service';
import { of, throwError } from 'rxjs';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

fdescribe('TaskListDeleted', () => {
  let component: TaskListDeleted;
  let fixture: ComponentFixture<TaskListDeleted>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;

  const mockDeletedTasks = [
    { id: '1', title: 'Deleted Task 1', status: 'Deleted' },
    { id: '2', title: 'Deleted Task 2', status: 'Deleted' }
  ];

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('TaskService', ['getDeletedTasks']);

    await TestBed.configureTestingModule({
      imports: [TaskListDeleted, CommonModule, RouterModule],
      providers: [
        { provide: TaskService, useValue: spy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(TaskListDeleted);
    component = fixture.componentInstance;
    taskServiceSpy = TestBed.inject(TaskService) as jasmine.SpyObj<TaskService>;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load deleted tasks on init (non-empty)', () => {
    spyOn(console, 'log');
    taskServiceSpy.getDeletedTasks.and.returnValue(of({ data: mockDeletedTasks }));

    component.ngOnInit();

    expect(taskServiceSpy.getDeletedTasks).toHaveBeenCalled();
    expect(component.deletedTasks.length).toBe(2);
    expect(component.deletedTasks[0].title).toBe('Deleted Task 1');
    expect(console.log).toHaveBeenCalledWith('🗑️ Deleted tasks:', mockDeletedTasks);
  });

  it('should handle empty response gracefully', () => {
    taskServiceSpy.getDeletedTasks.and.returnValue(of({ data: [] }));

    component.ngOnInit();

    expect(component.deletedTasks.length).toBe(0);
  });

  it('should handle error while fetching deleted tasks', () => {
    const error = { message: 'Server error' };
    spyOn(console, 'error');
    taskServiceSpy.getDeletedTasks.and.returnValue(throwError(() => error));

    component.ngOnInit();

    expect(taskServiceSpy.getDeletedTasks).toHaveBeenCalled();
    expect(console.error).toHaveBeenCalledWith('❌ Failed to fetch deleted tasks', error);
  });
});
