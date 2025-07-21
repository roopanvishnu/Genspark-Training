import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

import { LeaveTypeService } from '../../services/leave-type.service';
import { LeaveType } from '../../models/leave-type.model';

@Component({
  selector: 'app-leave-types',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './leave-types.html',
  styleUrls: ['./leave-types.css']
})
export class LeaveTypes implements OnInit {
  leaveTypes: LeaveType[] = [];
  filteredLeaveTypes: LeaveType[] = [];

  isHR = false;
  showForm = false;
  isEditing = false;
  isTableView = false;

  searchTerm = '';
  private searchSubject = new Subject<string>();

  selectedLeaveType: LeaveType = {
    name: '',
    standardLeaveCount: 0,
    description: ''
  };

  constructor(
    private leaveTypeService: LeaveTypeService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.checkUserRole();
    this.loadLeaveTypes();
    this.searchSubject.pipe(debounceTime(300)).subscribe(term => this.applyFilter(term));
  }

  checkUserRole() {
    this.isHR = localStorage.getItem('role') === 'HR';
  }

  loadLeaveTypes() {
    this.leaveTypeService.getLeaveTypes().subscribe({
      next: (data: LeaveType[]) => {
        this.leaveTypes = data;
        this.filteredLeaveTypes = data;
      },
      error: err => {
        this.toastr.error('Error fetching leave types', 'Error');
        console.error('Error fetching leave types:', err);
      }
    });
  }

  addNew() {
    this.selectedLeaveType = {
      name: '',
      standardLeaveCount: 0,
      description: ''
    };
    this.isEditing = false;
    this.showForm = true;
  }

  editLeaveType(lt: LeaveType) {
    this.selectedLeaveType = { ...lt };
    this.isEditing = true;
    this.showForm = true;
  }

  deleteLeaveType(id: string | undefined) {
    if (!id) return;

    if (!confirm('Are you sure you want to delete this leave type?')) return;

    this.leaveTypeService.deleteLeaveType(id).subscribe({
      next: () => {
        this.toastr.success('Leave type deleted successfully!', 'Deleted');
        this.loadLeaveTypes();
      },
      error: () => {
        this.toastr.error('Failed to delete leave type', 'Error');
      }
    });
  }

  saveLeaveType() {
    const { name, standardLeaveCount, id } = this.selectedLeaveType;

    if (!name || standardLeaveCount <= 0) {
      this.toastr.warning('Name and Max Days/Year must be valid.', 'Validation Error');
      return;
    }

    const saveObs = this.isEditing && id
      ? this.leaveTypeService.updateLeaveType(id, this.selectedLeaveType)
      : this.leaveTypeService.addLeaveType(this.selectedLeaveType);

    saveObs.subscribe({
      next: () => {
        this.toastr.success(
          `Leave type ${this.isEditing ? 'updated' : 'added'} successfully!`,
          this.isEditing ? 'Updated' : 'Added'
        );
        this.loadLeaveTypes();
        this.showForm = false;
      },
      error: () => {
        this.toastr.error(`Failed to ${this.isEditing ? 'update' : 'add'} leave type`, 'Error');
      }
    });
  }

  cancelForm() {
    this.showForm = false;
  }

  onSearchInputChange() {
    this.searchSubject.next(this.searchTerm);
  }

  applyFilter(term: string) {
    const lowerTerm = term.toLowerCase();
    this.filteredLeaveTypes = this.leaveTypes.filter(lt =>
      lt.name.toLowerCase().includes(lowerTerm) ||
      (lt.description?.toLowerCase().includes(lowerTerm))
    );
  }

  highlight(text: string): string {
    if (!this.searchTerm) return text;
    const re = new RegExp(`(${this.searchTerm})`, 'gi');
    return text.replace(re, `<mark class="bg-yellow-200">$1</mark>`);
  }
}
