import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { TaskService } from '../services/task.service';
import { ManagerService } from '../services/manager.service';
import { Task, TaskStatus, TaskPriority, CreateTaskRequest, UpdateTaskRequest, TaskFilter } from '../models/task.model';
import { User } from '../models/user.model';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl : './task-list.component.html'
})
export class TaskListComponent implements OnInit {
  tasks: Task[] = [];
  filteredTasks: Task[] = [];
  users: User[] = [];
  
  filter: TaskFilter = {};
  
  showCreateTaskModal = false;
  showAssignModal = false;
  selectedTask: Task | null = null;
  selectedAssigneeId = '';
  assignToAll = false;
  
  newTask: CreateTaskRequest = {
    title: '',
    description: '',
    priority: TaskPriority.Medium,
    isAssignedToAll: false
  };
  newTaskDueDate = '';

  constructor(
    public authService: AuthService,
    private taskService: TaskService,
    private managerService: ManagerService
  ) {}

  ngOnInit(): void {
    this.loadTasks();
    this.loadUsers();
  }

  loadTasks(): void {
    this.taskService.getTasks().subscribe(tasks => {
      this.tasks = tasks;
      this.applyFilters();
    });
  }

  loadUsers(): void {
    if (this.authService.isManager()) {
      this.managerService.getUsers().subscribe(users => {
        this.users = users.filter(u => u.isActive);
      });
    }
  }

  applyFilters(): void {
    this.filteredTasks = this.tasks.filter(task => {
      if (this.filter.status && task.status !== this.filter.status) return false;
      if (this.filter.priority && task.priority !== this.filter.priority) return false;
      if (this.filter.assigneeId) {
        if (this.filter.assigneeId === 'unassigned' && task.assigneeId) return false;
        if (this.filter.assigneeId !== 'unassigned' && task.assigneeId !== this.filter.assigneeId) return false;
      }
      if (this.filter.search) {
        const searchLower = this.filter.search.toLowerCase();
        if (!task.title.toLowerCase().includes(searchLower) && 
            !task.description.toLowerCase().includes(searchLower)) return false;
      }
      return true;
    });
  }

  createTask(): void {
    if (this.newTaskDueDate) {
      this.newTask.dueDate = new Date(this.newTaskDueDate);
    }

    this.taskService.createTask(this.newTask).subscribe(() => {
      this.showCreateTaskModal = false;
      this.resetNewTask();
      this.loadTasks();
    });
  }

  editTask(task: Task): void {
    // Implementation for editing task
    console.log('Edit task:', task);
  }

  deleteTask(taskId: string): void {
    if (confirm('Are you sure you want to delete this task?')) {
      this.taskService.deleteTask(taskId).subscribe(() => {
        this.loadTasks();
      });
    }
  }

  updateTaskStatus(task: Task, status: TaskStatus): void {
    const updateRequest: UpdateTaskRequest = { status };
    this.taskService.updateTask(task.id, updateRequest).subscribe(() => {
      this.loadTasks();
    });
  }

  showAssignTaskModal(task: Task): void {
    this.selectedTask = task;
    this.selectedAssigneeId = task.assigneeId || '';
    this.assignToAll = task.isAssignedToAll;
    this.showAssignModal = true;
  }

  assignTask(): void {
    if (!this.selectedTask) return;

    const updateRequest: UpdateTaskRequest = {
      assigneeId: this.assignToAll ? undefined : this.selectedAssigneeId
    };

    this.taskService.updateTask(this.selectedTask.id, updateRequest).subscribe(() => {
      this.showAssignModal = false;
      this.loadTasks();
    });
  }

  private resetNewTask(): void {
    this.newTask = {
      title: '',
      description: '',
      priority: TaskPriority.Medium,
      isAssignedToAll: false
    };
    this.newTaskDueDate = '';
  }
}