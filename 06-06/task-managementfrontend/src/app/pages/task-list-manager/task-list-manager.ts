import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { TaskService } from '../../services/task.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-task-list-manager',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl : './task-list-manager.html',
  styleUrl :'./task-list-manager.css'
})
export class TaskListManager implements OnInit {
  tasks: any[] = [];
  currentPage = 1;
  totalPages = 1;
  limit = 10;

  constructor(
    private taskService: TaskService,
    private router: Router
  ) {}

  ngOnInit() {
    this.fetchTasks(this.currentPage);
  }

  fetchTasks(page: number) {
  this.taskService.getAllTasks(page, this.limit).subscribe({
    next: res => {
      this.tasks = (res.data || []).filter((task: { isDeleted: any; }) => !task.isDeleted); // ✅ filter out deleted tasks
      this.currentPage = page;
      this.totalPages = Math.ceil(res.pagination.totalRecords / this.limit);
      console.log(`Page ${page} of ${this.totalPages}`, this.tasks);
      console.log('Full response from backend:', res);
    },
    error: err => {
      console.error('Failed to fetch tasks:', err);
    }
  });
}
deleteTask(taskId: string) {
  const confirmDelete = confirm('Are you sure you want to delete this task?');

  if (!confirmDelete) return;

  this.taskService.deleteTask(taskId).subscribe({
    next: () => {
      alert('🗑️ Task deleted successfully');
      this.fetchTasks(this.currentPage); // Refresh list
    },
    error: (err) => {
      console.error('❌ Failed to delete task:', err);
      alert('Failed to delete task');
    }
  });
}
goToDeletedTasks() {
  this.router.navigate(['/manager/tasks/deleted']);
}

  getPages(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  editTask(taskId: string) {
    console.log('Edit task', taskId);
    this.router.navigate([`/manager/tasks/${taskId}/edit`]);
  }

  assignTask(taskId: string) {
    this.router.navigate([`/manager/tasks/${taskId}/assign`]);
  }

  broadcastTask(taskId: string) {
    this.taskService.broadcastTask(taskId).subscribe({
      next: () => alert('✅ Task broadcasted!'),
      error: () => alert('❌ Failed to broadcast task'),
    });
  }

  viewAttachments(taskId: string) {
  this.taskService.getTaskAttachment(taskId).subscribe({
    next: (res) => {
      const blob = res.body!;
      const contentDisposition = res.headers.get('content-disposition');
      const filenameMatch = contentDisposition?.match(/filename="?(.+)"?/);
      const filename = filenameMatch?.[1] || 'attachment';

      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = filename;
      a.click();
      window.URL.revokeObjectURL(url);
    },
    error: (err) => {
      if (err.status === 404) {
        alert('❌ Attachment not found');
      } else {
        alert('❌ Failed to download attachment');
        console.error(err);
      }
    }
  });
}

}