import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AssignTask } from './assign-task';
import { UserService } from '../../services/user.service';
import { TaskService } from '../../services/task.service';
import { ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

fdescribe('AssignTask Component', () => {
  let component: AssignTask;
  let fixture: ComponentFixture<AssignTask>;
  let userServiceSpy: jasmine.SpyObj<UserService>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;

  beforeEach(async () => {
    const userSpy = jasmine.createSpyObj('UserService', ['getTeamMembers']);
    const taskSpy = jasmine.createSpyObj('TaskService', ['assignTask']);

    await TestBed.configureTestingModule({
      imports: [AssignTask, CommonModule, FormsModule],
      providers: [
        { provide: UserService, useValue: userSpy },
        { provide: TaskService, useValue: taskSpy },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: {
                get: (key: string) => (key === 'id' ? 'test-task-id' : null)
              }
            }
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AssignTask);
    component = fixture.componentInstance;
    userServiceSpy = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
    taskServiceSpy = TestBed.inject(TaskService) as jasmine.SpyObj<TaskService>;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should get taskId from route and fetch team members on init', () => {
    const mockUsers = {
      data: [
        { id: '1', name: 'A', role: 'Manager' },
        { id: '2', name: 'B', role: 'TeamMember' }
      ]
    };

    userServiceSpy.getTeamMembers.and.returnValue(of(mockUsers));

    component.ngOnInit();

    expect(component.taskId).toBe('test-task-id');
    expect(userServiceSpy.getTeamMembers).toHaveBeenCalled();
    expect(component.teamMembers.length).toBe(1); 
    expect(component.teamMembers[0].role).toBe('TeamMember');
  });

  it('should handle error if team members loading fails', () => {
    spyOn(console, 'log');
    userServiceSpy.getTeamMembers.and.returnValue(throwError(() => new Error('API failed')));

    component.ngOnInit();

    expect(console.log).toHaveBeenCalledWith('failed to load team member', jasmine.anything());
  });

  it('should not assign task if selectedUserId is empty', () => {
    component.selectedUserId = '';
    component.taskId = 'some-task-id';

    component.assign();

    expect(taskServiceSpy.assignTask).not.toHaveBeenCalled();
  });

  it('should assign task if selectedUserId is set', () => {
    spyOn(window, 'alert');
    component.selectedUserId = 'user-123';
    component.taskId = 'task-123';

    taskServiceSpy.assignTask.and.returnValue(of({}));

    component.assign();

    expect(taskServiceSpy.assignTask).toHaveBeenCalledWith('task-123', 'user-123');
    expect(window.alert).toHaveBeenCalledWith('Task assigned!');
  });

  it('should show error alert on assignment failure', () => {
    spyOn(window, 'alert');
    component.selectedUserId = 'user-456';
    component.taskId = 'task-456';

    taskServiceSpy.assignTask.and.returnValue(throwError(() => new Error('fail')));

    component.assign();

    expect(window.alert).toHaveBeenCalledWith('Assignment failed');
  });
});
