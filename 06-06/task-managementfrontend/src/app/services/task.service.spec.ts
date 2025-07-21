import { TestBed } from '@angular/core/testing';
import { TaskService } from './task.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

fdescribe('TaskService', () => {
  let service: TaskService;
  let httpMock: HttpTestingController;

  const baseUrl = 'https://localhost:7120/api/v1/tasks';

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [TaskService]
    });

    service = TestBed.inject(TaskService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch all tasks with pagination', () => {
    service.getAllTasks(2, 5).subscribe();

    const req = httpMock.expectOne(`${baseUrl}?page=2&limit=5`);
    expect(req.request.method).toBe('GET');
    req.flush({});
  });

  it('should create a new task with FormData', () => {
    const dto = new FormData();
    dto.append('title', 'Task');

    service.createTask(dto).subscribe();

    const req = httpMock.expectOne(baseUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body instanceof FormData).toBeTrue();
    req.flush({});
  });

  it('should get task by ID', () => {
    service.getTaskById('123').subscribe();

    const req = httpMock.expectOne(`${baseUrl}/123`);
    expect(req.request.method).toBe('GET');
    req.flush({});
  });

  it('should update a task with FormData', () => {
  const dto = new FormData();
  dto.append('status', 'InProgress');

  service.updateTask('456', dto).subscribe();

  const req = httpMock.expectOne(`${baseUrl}/456`); // ✅ Fix here
  expect(req.request.method).toBe('PUT');
  expect(req.request.body instanceof FormData).toBeTrue();
  req.flush({});
});


  it('should broadcast a task', () => {
    service.broadcastTask('789').subscribe();

    const req = httpMock.expectOne(`${baseUrl}/789/broadcast`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({});
    req.flush({});
  });

  it('should get attachments', () => {
  service.getTaskAttachment('taskId123').subscribe();

  const req = httpMock.expectOne(`${baseUrl}/taskId123/download`);
  expect(req.request.method).toBe('GET');
  req.flush(new Blob());
});


  it('should assign a task to a user', () => {
    service.assignTask('taskId123', 'userId456').subscribe();

    const req = httpMock.expectOne(`${baseUrl}/taskId123/assign/userId456`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({});
    req.flush({});
  });

  it('should get assigned tasks', () => {
    service.getAssignedTasks().subscribe();

    const req = httpMock.expectOne(`${baseUrl}/assigned`);
    expect(req.request.method).toBe('GET');
    req.flush({});
  });

  it('should download attachment as Blob', () => {
    service.downloadAttachment('taskId789').subscribe(res => {
      expect(res instanceof Blob).toBeTrue();
    });

    const req = httpMock.expectOne(`${baseUrl}/taskId789/download`);
    expect(req.request.method).toBe('GET');
    expect(req.request.responseType).toBe('blob');
    req.flush(new Blob(['file content'], { type: 'text/plain' }));
  });

  it('should update task status', () => {
    const payload = { status: 'Completed', comment: 'done' };

    service.updateTaskStatus('999', payload).subscribe();

    const req = httpMock.expectOne(`${baseUrl}/999/update-status`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(payload);
    req.flush({});
  });
});
