
// src/app/components/task-form/task-form.ts
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TaskService } from '../../services/task.service';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-task-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './task-form.html',
  styleUrl: './task-form.css'
})
export class TaskForm implements OnInit {
  taskForm!: FormGroup;
  selectedFile: File | null = null;
  taskId: string | null = null;

  constructor(
    private fb: FormBuilder,
    private taskService: TaskService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    this.taskForm = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      status: ['Open', Validators.required],
      dueDate: ['']
    });

    this.taskId = this.route.snapshot.paramMap.get('id');

    if (this.taskId) {
      this.taskService.getTaskById(this.taskId).subscribe({
        next: (res) => {
          this.taskForm.patchValue({
            title: res.title,
            description: res.description,
            status: res.status,
            dueDate: res.dueDate ? res.dueDate.substring(0, 16) : ''
          });
        }
      });
    }
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
    }
  }

  onSubmit() {
    if (!this.taskForm.valid) return;

    const formData = new FormData();
    formData.append('title', this.taskForm.value.title!);
    formData.append('description', this.taskForm.value.description!);
    formData.append('status', this.taskForm.value.status!);
    formData.append('dueDate', this.taskForm.value.dueDate || '');

    if (this.selectedFile) {
      formData.append('attachment', this.selectedFile);
    }

    if (this.taskId) {
      this.taskService.updateTask(this.taskId, formData).subscribe({
        next: () => {
          alert('✅ Task updated successfully');
          this.router.navigate(['/manager/tasks']);
        },
        error: (err) => {
          console.error('❌ Update failed:', err);
          alert(err.error?.message || 'Task update failed');
        }
      });
    } else {
      this.taskService.createTask(formData).subscribe({
        next: () => {
          alert('✅ Task created successfully');
          this.router.navigate(['/manager/tasks']);
        },
        error: (err) => {
          console.error('❌ Creation failed:', err);
          alert(err.error?.message || 'Task creation failed');
        }
      });
    }
  }
}
