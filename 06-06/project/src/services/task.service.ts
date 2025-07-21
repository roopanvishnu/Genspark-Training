import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Task, TaskStatus, TaskPriority, CreateTaskRequest, UpdateTaskRequest, TaskFilter } from '../models/task.model';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private tasksSubject = new BehaviorSubject<Task[]>([]);
  public tasks$ = this.tasksSubject.asObservable();

  constructor(private authService: AuthService) {
    this.loadMockTasks();
  }

  getTasks(filter?: TaskFilter): Observable<Task[]> {
    return new Observable(observer => {
      setTimeout(() => {
        let tasks = this.tasksSubject.value;
        const currentUser = this.authService.getCurrentUser();
        
        // Filter tasks based on user role
        if (currentUser?.role === 'TeamMember') {
          tasks = tasks.filter(task => 
            task.assigneeId === currentUser.id || task.isAssignedToAll
          );
        }
        
        // Apply additional filters
        if (filter) {
          if (filter.status) {
            tasks = tasks.filter(task => task.status === filter.status);
          }
          if (filter.priority) {
            tasks = tasks.filter(task => task.priority === filter.priority);
          }
          if (filter.assigneeId) {
            tasks = tasks.filter(task => task.assigneeId === filter.assigneeId);
          }
          if (filter.search) {
            const searchLower = filter.search.toLowerCase();
            tasks = tasks.filter(task => 
              task.title.toLowerCase().includes(searchLower) ||
              task.description.toLowerCase().includes(searchLower)
            );
          }
        }
        
        observer.next(tasks);
        observer.complete();
      }, 500);
    });
  }

  createTask(request: CreateTaskRequest): Observable<Task> {
    return new Observable(observer => {
      setTimeout(() => {
        const currentUser = this.authService.getCurrentUser();
        if (!currentUser) {
          observer.error({ message: 'User not authenticated' });
          return;
        }

        const newTask: Task = {
          id: Math.random().toString(),
          title: request.title,
          description: request.description,
          status: TaskStatus.ToDo,
          priority: request.priority,
          assigneeId: request.assigneeId,
          assignee: request.assigneeId ? {
            id: request.assigneeId,
            firstName: 'Assigned',
            lastName: 'User',
            email: 'user@demo.com'
          } : undefined,
          createdById: currentUser.id,
          createdBy: {
            id: currentUser.id,
            firstName: currentUser.firstName,
            lastName: currentUser.lastName
          },
          createdAt: new Date(),
          updatedAt: new Date(),
          dueDate: request.dueDate,
          attachments: [],
          isAssignedToAll: request.isAssignedToAll
        };

        const currentTasks = this.tasksSubject.value;
        this.tasksSubject.next([...currentTasks, newTask]);
        observer.next(newTask);
        observer.complete();
      }, 800);
    });
  }

  updateTask(taskId: string, request: UpdateTaskRequest): Observable<Task> {
    return new Observable(observer => {
      setTimeout(() => {
        const tasks = this.tasksSubject.value;
        const taskIndex = tasks.findIndex(t => t.id === taskId);
        
        if (taskIndex === -1) {
          observer.error({ message: 'Task not found' });
          return;
        }

        const updatedTask = {
          ...tasks[taskIndex],
          ...request,
          updatedAt: new Date()
        };

        tasks[taskIndex] = updatedTask;
        this.tasksSubject.next([...tasks]);
        observer.next(updatedTask);
        observer.complete();
      }, 500);
    });
  }

  deleteTask(taskId: string): Observable<void> {
    return new Observable(observer => {
      setTimeout(() => {
        const tasks = this.tasksSubject.value;
        const filteredTasks = tasks.filter(t => t.id !== taskId);
        this.tasksSubject.next(filteredTasks);
        observer.next();
        observer.complete();
      }, 500);
    });
  }

  private loadMockTasks(): void {
    const mockTasks: Task[] = [
      {
        id: '1',
        title: 'Fix UI Bug in Dashboard',
        description: 'The dashboard cards are not displaying correctly on mobile devices. Need to fix responsive layout.',
        status: TaskStatus.InProgress,
        priority: TaskPriority.High,
        assigneeId: '2',
        assignee: {
          id: '2',
          firstName: 'Roopan',
          lastName: 'Vishnu',
          email: 'member@demo.com'
        },
        createdById: '1',
        createdBy: {
          id: '1',
          firstName: 'John',
          lastName: 'Manager'
        },
        createdAt: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000),
        updatedAt: new Date(Date.now() - 1 * 60 * 60 * 1000),
        dueDate: new Date(Date.now() + 3 * 24 * 60 * 60 * 1000),
        attachments: [
          {
            id: '1',
            fileName: 'mobile-screenshot.png',
            fileSize: 256000,
            fileType: 'image/png',
            uploadedAt: new Date(),
            downloadUrl: '#'
          }
        ],
        isAssignedToAll: false
      },
      {
        id: '2',
        title: 'Deploy to Production',
        description: 'Deploy the latest version of the application to production environment after thorough testing.',
        status: TaskStatus.Review,
        priority: TaskPriority.Critical,
        assigneeId: undefined,
        createdById: '1',
        createdBy: {
          id: '1',
          firstName: 'John',
          lastName: 'Manager'
        },
        createdAt: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000),
        updatedAt: new Date(Date.now() - 30 * 60 * 1000),
        dueDate: new Date(Date.now() + 1 * 24 * 60 * 60 * 1000),
        attachments: [],
        isAssignedToAll: true
      },
      {
        id: '3',
        title: 'Update Documentation',
        description: 'Update API documentation with the latest endpoints and examples.',
        status: TaskStatus.ToDo,
        priority: TaskPriority.Medium,
        assigneeId: '2',
        assignee: {
          id: '2',
          firstName: 'Roopan',
          lastName: 'Vishnu',
          email: 'member@demo.com'
        },
        createdById: '1',
        createdBy: {
          id: '1',
          firstName: 'John',
          lastName: 'Manager'
        },
        createdAt: new Date(Date.now() - 12 * 60 * 60 * 1000),
        updatedAt: new Date(Date.now() - 12 * 60 * 60 * 1000),
        dueDate: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000),
        attachments: [],
        isAssignedToAll: false
      },
      {
        id: '4',
        title: 'Security Audit',
        description: 'Conduct a comprehensive security audit of the application and fix any vulnerabilities.',
        status: TaskStatus.Completed,
        priority: TaskPriority.High,
        assigneeId: undefined,
        createdById: '1',
        createdBy: {
          id: '1',
          firstName: 'John',
          lastName: 'Manager'
        },
        createdAt: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000),
        updatedAt: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000),
        dueDate: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000),
        attachments: [
          {
            id: '2',
            fileName: 'security-audit-report.pdf',
            fileSize: 1024000,
            fileType: 'application/pdf',
            uploadedAt: new Date(),
            downloadUrl: '#'
          }
        ],
        isAssignedToAll: true
      }
    ];

    this.tasksSubject.next(mockTasks);
  }
}