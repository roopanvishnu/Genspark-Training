import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { UserDetailDto } from '../../../models/user-detail-dto.model';
import { UserService } from '../../../services/user.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-user-detail',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './user-detail.html',
})
export class UserDetailComponent implements OnInit {
  userDetail!: UserDetailDto;
  userId!: string;

  page = 1;
  pageSize = 5;
  totalPages = 1;

  pagedLeaveRequests: any[] = [];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private userService: UserService
  ) {}

  ngOnInit(): void {
    this.userId = this.route.snapshot.params['id'];
    this.loadUserDetail();
  }

  loadUserDetail() {
    this.userService.getFullUserDetail(this.userId, this.page, this.pageSize).subscribe({
      next: (res: any) => {
        const raw = res;

        this.userDetail = {
          ...raw,
          leaveBalances: raw.leaveBalances?.$values ?? [],
          leaveRequests: raw.leaveRequests?.$values ?? [],
        };

        this.updatePagination();
      },
      error: (err: any) => {
        console.error('Failed to load user detail', err);
      },
    });
  }

  updatePagination() {
    const total = this.userDetail.leaveRequests.length;
    this.totalPages = Math.ceil(total / this.pageSize);
    const start = (this.page - 1) * this.pageSize;
    const end = this.page * this.pageSize;
    this.pagedLeaveRequests = this.userDetail.leaveRequests.slice(start, end);
  }

  prevPage() {
    if (this.page > 1) {
      this.page--;
      this.updatePagination();
    }
  }

  nextPage() {
    if (this.page < this.totalPages) {
      this.page++;
      this.updatePagination();
    }
  }

  goBack() {
    this.router.navigate(['/user-management']);
  }
}
