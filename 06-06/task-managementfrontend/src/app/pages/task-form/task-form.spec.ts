import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { TaskForm } from './task-form';
import { TaskService } from '../../services/task.service';

fdescribe('TaskForm', () => {
  let component: TaskForm;
  let fixture: ComponentFixture<TaskForm>;
  let mockTaskService: jasmine.SpyObj<TaskService>;
  let mockRouter: jasmine.SpyObj<Router>;
  let mockActivatedRoute: any;

  const mockTaskData = {
    id: 'task123',
    title: 'Test Task',
    description: 'Test Description',
    status: 'In Progress',
    deadline: '2024-12-31T23:59:00.000Z'
  };

  beforeEach(async () => {
    const taskServiceSpy = jasmine.createSpyObj('TaskService', [
      'getTaskById',
      'createTask',
      'updateTask'
    ]);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    const activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get')
        }
      }
    };

    await TestBed.configureTestingModule({
      imports: [TaskForm, ReactiveFormsModule],
      providers: [
        FormBuilder,
        { provide: TaskService, useValue: taskServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: ActivatedRoute, useValue: activatedRouteMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(TaskForm);
    component = fixture.componentInstance;
    mockTaskService = TestBed.inject(TaskService) as jasmine.SpyObj<TaskService>;
    mockRouter = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    mockActivatedRoute = TestBed.inject(ActivatedRoute);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Component Initialization', () => {
    it('should initialize with default values', () => {
      expect(component.selectedFile).toBeNull();
      expect(component.taskId).toBeNull();
    });

    it('should create form with default values for new task', () => {
  mockActivatedRoute.snapshot.paramMap.get.and.returnValue(null);
  
  component.ngOnInit();

  expect(component.taskForm).toBeDefined();
  expect(component.taskForm.get('title')?.value).toBe('');
  expect(component.taskForm.get('description')?.value).toBe('');
  expect(component.taskForm.get('status')?.value).toBe('Open');
  expect(component.taskForm.get('dueDate')?.value).toBe('');
});

    it('should have required validators on title and description', () => {
      mockActivatedRoute.snapshot.paramMap.get.and.returnValue(null);
      
      component.ngOnInit();

      const titleControl = component.taskForm.get('title');
      const descriptionControl = component.taskForm.get('description');
      const statusControl = component.taskForm.get('status');

      expect(titleControl?.hasError('required')).toBeTruthy();
      expect(descriptionControl?.hasError('required')).toBeTruthy();
      expect(statusControl?.hasError('required')).toBeFalsy(); // has default value
    });
  });

  describe('File Selection', () => {
    it('should handle file selection', () => {
      const mockFile = new File(['test content'], 'test.pdf', { type: 'application/pdf' });
      const mockEvent = {
        target: {
          files: [mockFile]
        }
      };

      component.onFileSelected(mockEvent);

      expect(component.selectedFile).toBe(mockFile);
    });

    it('should handle empty file selection', () => {
      const mockEvent = {
        target: {
          files: []
        }
      };

      component.onFileSelected(mockEvent);

      expect(component.selectedFile).toBeNull();
    });

    it('should handle null file in selection', () => {
      const mockEvent = {
        target: {
          files: [null]
        }
      };

      component.onFileSelected(mockEvent);

      expect(component.selectedFile).toBeNull();
    });
  });

  describe('Form Submission - Create Task', () => {
    beforeEach(() => {
      mockActivatedRoute.snapshot.paramMap.get.and.returnValue(null);
      component.ngOnInit();
    });

    it('should not submit invalid form', () => {
      // Leave form empty (invalid)
      component.onSubmit();

      expect(mockTaskService.createTask).not.toHaveBeenCalled();
      expect(mockTaskService.updateTask).not.toHaveBeenCalled();
    });

    it('should create task successfully without file', () => {
  spyOn(window, 'alert');
  mockTaskService.createTask.and.returnValue(of({}));
  
  component.taskForm.patchValue({
    title: 'New Task',
    description: 'New Description',
    status: 'Open',
    dueDate: '2024-12-31T10:00'
  });

  component.onSubmit();

  expect(mockTaskService.createTask).toHaveBeenCalled();
  const formDataArg = mockTaskService.createTask.calls.mostRecent().args[0] as FormData;
  expect(formDataArg.get('title')).toBe('New Task');
  expect(formDataArg.get('description')).toBe('New Description');
  expect(formDataArg.get('status')).toBe('Open');
  expect(formDataArg.get('dueDate')).toBe('2024-12-31T10:00');
  expect(formDataArg.get('attachment')).toBeNull();
  
  expect(window.alert).toHaveBeenCalledWith('✅ Task created successfully');
  expect(mockRouter.navigate).toHaveBeenCalledWith(['/manager/tasks']);
});


    it('should create task successfully with file', () => {
      spyOn(window, 'alert');
      mockTaskService.createTask.and.returnValue(of({}));
      const mockFile = new File(['test'], 'test.pdf', { type: 'application/pdf' });
      
      component.taskForm.patchValue({
        title: 'New Task',
        description: 'New Description',
        status: 'Open'
      });
      component.selectedFile = mockFile;

      component.onSubmit();

      const formDataArg = mockTaskService.createTask.calls.mostRecent().args[0] as FormData;
      expect(formDataArg.get('attachment')).toBe(mockFile);
    });

    it('should handle empty deadline in create task', () => {
  spyOn(window, 'alert');
  mockTaskService.createTask.and.returnValue(of({}));
  
  component.taskForm.patchValue({
    title: 'New Task',
    description: 'New Description',
    status: 'Open',
    dueDate: '' // empty dueDate instead of 'deadline'
  });

  component.onSubmit();

  const formDataArg = mockTaskService.createTask.calls.mostRecent().args[0] as FormData;
  expect(formDataArg.get('dueDate')).toBe('');
});


    it('should handle create task error', () => {
      spyOn(window, 'alert');
      spyOn(console, 'error');
      const error = { error: { message: 'Creation failed' } };
      mockTaskService.createTask.and.returnValue(throwError(() => error));
      
      component.taskForm.patchValue({
        title: 'New Task',
        description: 'New Description'
      });

      component.onSubmit();

      expect(console.error).toHaveBeenCalledWith('❌ Creation failed:', error);
      expect(window.alert).toHaveBeenCalledWith('Creation failed');
      expect(mockRouter.navigate).not.toHaveBeenCalled();
    });

    it('should handle create task error without specific message', () => {
      spyOn(window, 'alert');
      const error = { error: {} };
      mockTaskService.createTask.and.returnValue(throwError(() => error));
      
      component.taskForm.patchValue({
        title: 'New Task',
        description: 'New Description'
      });

      component.onSubmit();

      expect(window.alert).toHaveBeenCalledWith('Task creation failed');
    });
  });

  describe('Form Submission - Update Task', () => {
    beforeEach(() => {
      mockActivatedRoute.snapshot.paramMap.get.and.returnValue('task123');
      mockTaskService.getTaskById.and.returnValue(of(mockTaskData));
      component.ngOnInit();
    });

    it('should update task successfully', () => {
      spyOn(window, 'alert');
      mockTaskService.updateTask.and.returnValue(of({}));
      
      component.taskForm.patchValue({
        title: 'Updated Task',
        description: 'Updated Description',
        status: 'Completed'
      });

      component.onSubmit();

      expect(mockTaskService.updateTask).toHaveBeenCalledWith('task123', jasmine.any(FormData));
      const formDataArg = mockTaskService.updateTask.calls.mostRecent().args[1] as FormData;
      expect(formDataArg.get('title')).toBe('Updated Task');
      expect(formDataArg.get('description')).toBe('Updated Description');
      expect(formDataArg.get('status')).toBe('Completed');
      
      expect(window.alert).toHaveBeenCalledWith('✅ Task updated successfully');
      expect(mockRouter.navigate).toHaveBeenCalledWith(['/manager/tasks']);
    });

    it('should update task with file attachment', () => {
      spyOn(window, 'alert');
      mockTaskService.updateTask.and.returnValue(of({}));
      const mockFile = new File(['updated content'], 'updated.pdf');
      
      component.selectedFile = mockFile;
      component.onSubmit();

      const formDataArg = mockTaskService.updateTask.calls.mostRecent().args[1] as FormData;
      expect(formDataArg.get('attachment')).toBe(mockFile);
    });

    it('should handle update task error', () => {
      spyOn(window, 'alert');
      spyOn(console, 'error');
      const error = { error: { message: 'Update failed' } };
      mockTaskService.updateTask.and.returnValue(throwError(() => error));

      component.onSubmit();

      expect(console.error).toHaveBeenCalledWith('❌ Update failed:', error);
      expect(window.alert).toHaveBeenCalledWith('Update failed');
      expect(mockRouter.navigate).not.toHaveBeenCalled();
    });

    it('should handle update task error without specific message', () => {
      spyOn(window, 'alert');
      const error = { error: {} };
      mockTaskService.updateTask.and.returnValue(throwError(() => error));

      component.onSubmit();

      expect(window.alert).toHaveBeenCalledWith('Task update failed');
    });
  });

  describe('Form Validation', () => {
    beforeEach(() => {
      mockActivatedRoute.snapshot.paramMap.get.and.returnValue(null);
      component.ngOnInit();
    });

    it('should validate required fields', () => {
      // Initially form should be invalid
      expect(component.taskForm.valid).toBeFalsy();
      expect(component.taskForm.get('title')?.hasError('required')).toBeTruthy();
      expect(component.taskForm.get('description')?.hasError('required')).toBeTruthy();

      // Fill required fields
      component.taskForm.patchValue({
        title: 'Valid Title',
        description: 'Valid Description'
      });

      expect(component.taskForm.valid).toBeTruthy();
      expect(component.taskForm.get('title')?.hasError('required')).toBeFalsy();
      expect(component.taskForm.get('description')?.hasError('required')).toBeFalsy();
    });

    it('should allow optional deadline field', () => {
      component.taskForm.patchValue({
        title: 'Valid Title',
        description: 'Valid Description',
        deadline: '' // empty deadline should be valid
      });

      expect(component.taskForm.valid).toBeTruthy();
    });
  });

  describe('Integration Tests', () => {
    it('should handle complete create workflow', () => {
      spyOn(window, 'alert');
      mockActivatedRoute.snapshot.paramMap.get.and.returnValue(null);
      mockTaskService.createTask.and.returnValue(of({ id: 'new-task-id' }));

      component.ngOnInit();
      
      // Fill form
      component.taskForm.patchValue({
        title: 'Integration Test Task',
        description: 'Integration Test Description',
        status: 'Open',
        deadline: '2024-12-31T15:30'
      });

      // Add file
      const mockFile = new File(['test'], 'test.txt');
      component.selectedFile = mockFile;

      // Submit
      component.onSubmit();

      expect(mockTaskService.createTask).toHaveBeenCalled();
      expect(window.alert).toHaveBeenCalledWith('✅ Task created successfully');
      expect(mockRouter.navigate).toHaveBeenCalledWith(['/manager/tasks']);
    });

    it('should handle complete update workflow', () => {
      spyOn(window, 'alert');
      mockActivatedRoute.snapshot.paramMap.get.and.returnValue('existing-task');
      mockTaskService.getTaskById.and.returnValue(of(mockTaskData));
      mockTaskService.updateTask.and.returnValue(of({}));

      component.ngOnInit();
      
      // Modify loaded data
      component.taskForm.patchValue({
        title: 'Updated Integration Test Task'
      });

      // Submit
      component.onSubmit();

      expect(mockTaskService.getTaskById).toHaveBeenCalledWith('existing-task');
      expect(mockTaskService.updateTask).toHaveBeenCalledWith('existing-task', jasmine.any(FormData));
      expect(window.alert).toHaveBeenCalledWith('✅ Task updated successfully');
      expect(mockRouter.navigate).toHaveBeenCalledWith(['/manager/tasks']);
    });
  });
});