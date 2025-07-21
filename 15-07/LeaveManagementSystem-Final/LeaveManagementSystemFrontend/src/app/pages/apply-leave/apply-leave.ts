import { Component, OnInit, inject } from '@angular/core';
import {
  FormBuilder,
  Validators,
  FormGroup,
  ReactiveFormsModule
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

import { LeaveTypeService } from '../../services/leave-type.service';
import { LeaveRequestService } from '../../services/leave-request.service';
import { LeaveAttachmentService } from '../../services/leave-attachment.service';

import { LeaveType } from '../../models/leave-type.model';
import { LeaveRequestDto } from '../../models/leave-request.dto';

@Component({
  selector: 'app-apply-leave',
  standalone: true,
  templateUrl: './apply-leave.html',
  styleUrls: ['./apply-leave.css'],
  imports: [CommonModule, ReactiveFormsModule]
})
export class ApplyLeave implements OnInit {
  applyLeaveForm!: FormGroup;
  leaveTypes: LeaveType[] = [];
  filesToUpload: File[] = [];

  private toastr = inject(ToastrService);

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private leaveTypeService: LeaveTypeService,
    private leaveRequestService: LeaveRequestService,
    private leaveAttachmentService: LeaveAttachmentService
  ) {}

  ngOnInit(): void {
    this.applyLeaveForm = this.fb.group({
      leaveTypeId: ['', Validators.required],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
      reason: ['', Validators.required],
      files: [null]
    });

    this.applyLeaveForm.get('startDate')?.valueChanges.subscribe(start => {
      const end = this.applyLeaveForm.get('endDate')?.value;
      if (end && end < start) {
        this.applyLeaveForm.get('endDate')?.setErrors({ dateMismatch: true });
      } else {
        this.applyLeaveForm.get('endDate')?.updateValueAndValidity();
      }
    });

    this.loadLeaveTypes();
  }

  loadLeaveTypes() {
    this.leaveTypeService.getLeaveTypes().subscribe({
      next: (types) => {
        this.leaveTypes = types;
      },
      error: () => {
        this.toastr.error('Error loading leave types');
      }
    });
  }

  handleFileInput(event: Event) {
    const input = event.target as HTMLInputElement;
    this.filesToUpload = Array.from(input.files || []);
  }

  onSubmit() {
    this.applyLeaveForm.markAllAsTouched();
    if (this.applyLeaveForm.invalid) {
      this.toastr.warning('Please fill all required fields correctly');
      return;
    }

    const formValue = this.applyLeaveForm.value;

    const dto: LeaveRequestDto = {
      leaveTypeId: formValue.leaveTypeId,
      startDate: formValue.startDate,
      endDate: formValue.endDate,
      reason: formValue.reason
    };

    this.leaveRequestService.createLeaveRequest(dto).subscribe({
      next: (res) => {
        const leaveRequestId = res.data.id;

        if (this.filesToUpload.length === 0) {
          this.toastr.success('Leave applied successfully');
          this.router.navigate(['/leave-history']);
          return;
        }

        let uploaded = 0;

        this.filesToUpload.forEach(file => {
          this.leaveAttachmentService.uploadAttachment(leaveRequestId, file).subscribe({
            next: () => {
              uploaded++;
              if (uploaded === this.filesToUpload.length) {
                this.toastr.success('Leave and all attachments submitted');
                this.router.navigate(['/leave-history']);
              }
            },
            error: () => {
              this.toastr.error(`Failed to upload: ${file.name}`);
            }
          });
        });
      },
      error: (err) => {
        const msg = err?.error?.message || 'Failed to apply leave';
        this.toastr.error(msg);
      }
    });
  }
}
