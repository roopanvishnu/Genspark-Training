import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TaskService } from '../../services/task.service';

@Component({
  selector: 'app-task-list-deleted',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './task-list-deleted.html',
  styleUrl: './task-list-deleted.css' 
})
export class TaskListDeleted implements OnInit {
  deletedTasks: any[] = [];

  constructor(private taskService: TaskService) {}

  ngOnInit() {
    this.taskService.getDeletedTasks().subscribe({
      next: (res) => {
        this.deletedTasks = res.data || [];
        console.log('🗑️ Deleted tasks:', this.deletedTasks);
      },
      error: (err) => {
        console.error('❌ Failed to fetch deleted tasks', err);
      }
    });
  }
}
