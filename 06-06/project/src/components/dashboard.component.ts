import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../services/auth.service';
import { TaskService } from '../services/task.service';
import { SignalRService } from '../services/signalr.service';
import { User } from '../models/user.model';
import { Task, TaskStatus, TaskPriority } from '../models/task.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl : "./dashboard.component.html"
})
export class DashboardComponent implements OnInit {
  currentUser: User | null = null;
  tasks: Task[] = [];
  recentTasks: Task[] = [];
  notifications: string[] = [];
  
  taskStats = {
    total: 0,
    inProgress: 0,
    completed: 0,
    highPriority: 0
  };

  constructor(
    public authService: AuthService,
    private taskService: TaskService,
    private signalRService: SignalRService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    this.loadTasks();
    this.subscribeToNotifications();
  }

  loadTasks(): void {
    this.taskService.getTasks().subscribe(tasks => {
      this.tasks = tasks;
      this.recentTasks = tasks.slice(0, 5);
      this.calculateStats();
    });
  }

  subscribeToNotifications(): void {
    this.signalRService.notifications$.subscribe(notifications => {
      this.notifications = notifications;
    });
  }

  calculateStats(): void {
    this.taskStats = {
      total: this.tasks.length,
      inProgress: this.tasks.filter(t => t.status === TaskStatus.InProgress).length,
      completed: this.tasks.filter(t => t.status === TaskStatus.Completed).length,
      highPriority: this.tasks.filter(t => t.priority === TaskPriority.High || t.priority === TaskPriority.Critical).length
    };
  }

  getRoleBasedWelcomeMessage(): string {
    if (this.authService.isManager()) {
      return "Here's an overview of your team's tasks and recent activity.";
    } else {
      return "Here are your assigned tasks and recent updates.";
    }
  }

  clearNotifications(): void {
    this.signalRService.clearNotifications();
  }
}