import { Component, OnInit } from '@angular/core';
import { LeaveRequestService } from '../../services/leave-request.service';
import { LeaveRequestResponse } from '../../models/leave-request-response.model';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-leave-history',
  standalone: true,
  templateUrl: './leave-history.html',
  imports: [RouterLink, CommonModule, FormsModule]
})
export class LeaveHistory implements OnInit {
  allLeaves: LeaveRequestResponse[] = [];
  leaveRequests: LeaveRequestResponse[] = [];
  errorMsg: string = '';
  searchText: string = '';
  sortBy: string = 'startDate';
  sortDirection: string = 'asc';
  page: number = 1;
  pageSize: number = 10;
  totalItems: number = 0;

  sortOptions = [
    { value: 'createdAt', label: 'Created At' },
    { value: 'updatedAt', label: 'Updated At' },
    { value: 'startDate', label: 'Start Date' },
    { value: 'endDate', label: 'End Date' },
    { value: 'status', label: 'Status' }
  ];

  directionOptions = [
    { value: 'asc', label: 'Ascending' },
    { value: 'desc', label: 'Descending' }
  ];

  constructor(private leaveRequestService: LeaveRequestService) {}

  ngOnInit(): void {
    this.loadLeaveHistory();
  }

  get totalPages(): number {
    return Math.ceil(this.totalItems / this.pageSize);
  }

  loadLeaveHistory(): void {
    this.leaveRequestService.getAllLeaveRequests(1, 9999, '', '', this.sortBy, this.sortDirection)
      .subscribe({
        next: (res:any) => {
          const raw = res.data?.data?.$values || res.data?.$values || res.data || [];
          this.allLeaves = raw;
          this.applyFiltersAndPagination();
          this.errorMsg = '';
        },
        error: (err) => {
          this.errorMsg = 'Failed to load leave history.';
          console.error(err);
        }
      });
  }

  applyFiltersAndPagination(): void {
    let filtered = [...this.allLeaves];
    const id = localStorage.getItem('userId');
    if (id) {
      filtered = filtered.filter(leave => leave.userId === id);
    }

    if (this.searchText.trim()) {
      const search = this.searchText.toLowerCase();
      filtered = filtered.filter(l =>
        l.reason?.toLowerCase().includes(search) ||
        l.leaveTypeName?.toLowerCase().includes(search)
      );
    }

    filtered.sort((a, b) => {
      const aValue = (a as any)[this.sortBy];
      const bValue = (b as any)[this.sortBy];
      if (aValue < bValue) return this.sortDirection === 'asc' ? -1 : 1;
      if (aValue > bValue) return this.sortDirection === 'asc' ? 1 : -1;
      return 0;
    });

    this.totalItems = filtered.length;
    const start = (this.page - 1) * this.pageSize;
    const end = start + this.pageSize;
    this.leaveRequests = filtered.slice(start, end);
  }

  onSortChange(): void {
    this.page = 1;
    this.applyFiltersAndPagination();
  }

  previousPage(): void {
    if (this.page > 1) {
      this.page--;
      this.applyFiltersAndPagination();
    }
  }

  nextPage(): void {
    if (this.page < this.totalPages) {
      this.page++;
      this.applyFiltersAndPagination();
    }
  }
}
