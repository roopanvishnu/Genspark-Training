import { Component, OnInit } from '@angular/core';
import { LeaveRequestResponse } from '../../models/leave-request-response.model';
import { LeaveRequestService } from '../../services/leave-request.service';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-leave-approvals',
  templateUrl: './leave-approvals.html',
  styleUrls: ['./leave-approvals.css'],
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, RouterLink]
})
export class LeaveApprovals implements OnInit {
  leaveRequests: LeaveRequestResponse[] = [];
  filteredRequests: LeaveRequestResponse[] = [];
  errorMsg = '';
  searchTerm = '';
  page = 1;
  pageSize = 10;
  totalPages = 1;
  sortBy: keyof LeaveRequestResponse = 'createdAt';
  sortOrder: 'asc' | 'desc' = 'desc';
  isTableView = false;
  statusFilter: string = 'All';

  statusOptions = ['All', 'Pending', 'Approved', 'Rejected', 'Auto-Rejected', 'Cancelled'];

  sortOptions = [
    { value: 'createdAt', label: 'Created At' },
    { value: 'updatedAt', label: 'Updated At' },
    { value: 'startDate', label: 'Start Date' },
    { value: 'endDate', label: 'End Date' },
    { value: 'status', label: 'Status' }
  ];

  constructor(private leaveRequestService: LeaveRequestService) {}

  ngOnInit(): void {
    this.fetchData();
  }

  fetchData(): void {
    this.leaveRequestService.getAllLeaveRequests(
      1, 10000, '', '', this.sortBy, this.sortOrder
    ).subscribe({
      next: (res:any) => {
        this.leaveRequests = res?.data?.data?.$values || res?.data?.$values || [];
        this.filterRequests();
      },
      error: (err) => {
        this.errorMsg = 'Error loading leave approvals';
        console.error(err);
      }
    });
  }

  filterRequests(): void {
    const term = this.searchTerm.toLowerCase();

    const filtered = this.leaveRequests
      .filter(leave =>
        (leave.userName?.toLowerCase().includes(term) ||
         leave.reason?.toLowerCase().includes(term) ||
         leave.leaveTypeName?.toLowerCase().includes(term)) &&
        (this.statusFilter === 'All' || leave.status === this.statusFilter)
      )
      .sort((a, b) => {
        const aValue = a[this.sortBy] ?? '';
        const bValue = b[this.sortBy] ?? '';
        return this.sortOrder === 'asc'
          ? (aValue > bValue ? 1 : -1)
          : (aValue < bValue ? 1 : -1);
      });

    this.totalPages = Math.ceil(filtered.length / this.pageSize);
    const start = (this.page - 1) * this.pageSize;
    const end = start + this.pageSize;
    this.filteredRequests = filtered.slice(start, end);
  }

  onSearchChange(): void {
    this.page = 1;
    this.filterRequests();
  }

  onSortChange(): void {
    this.page = 1;
    this.filterRequests();
  }

  onStatusChange(): void {
    this.page = 1;
    this.filterRequests();
  }

  previousPage(): void {
    if (this.page > 1) {
      this.page--;
      this.filterRequests();
    }
  }

  nextPage(): void {
    if (this.page < this.totalPages) {
      this.page++;
      this.filterRequests();
    }
  }

  trackByLeaveId(index: number, leave: LeaveRequestResponse): string {
    return leave.id;
  }
}
