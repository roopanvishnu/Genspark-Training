
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TaskService } from '../../services/task.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-team-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl : './team-dashboard.html',
  styleUrl : './team-dashboard.css'
})
export class TeamDashboard implements OnInit {
  assignedTasks: any[] = [];

  constructor(private taskService: TaskService) {}

  ngOnInit(): void {
    this.taskService.getAssignedTasks().subscribe({
      next: (res) => {
        this.assignedTasks = res.data.map((task: any) => ({
          ...task,
          newStatus: task.status,
          newComment: ''
        }));
      },
      error: (err) => {
        console.error('❌ Failed to load assigned tasks', err);
      }
    });
  }

  updateTask(task: any) {
    if (!task.newStatus) return;

    this.taskService.updateTaskStatus(task.id, {
      status: task.newStatus,
      comment: task.newComment
    }).subscribe({
      next: () => {
        alert('✅ Task status updated');
        task.status = task.newStatus;
        task.newComment = '';
      },
      error: (err) => {
        console.error('❌ Failed to update status', err);
        alert('❌ Failed to update status');
      }
    });
  }

  downloadAttachment(taskId: string) {
    this.taskService.downloadAttachment(taskId).subscribe({
      next: (blob) => {
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = `task-${taskId}-attachment`;
        link.click();
        URL.revokeObjectURL(link.href);
      },
      error: (err) => {
        console.error('❌ Failed to download attachment', err);
        alert('No attachment found or access denied');
      }
    });
  }
}
