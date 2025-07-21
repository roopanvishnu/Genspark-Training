import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { TaskService } from '../../services/task.service';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-assign-task',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl : './assign-task.html',
  styleUrl : './assign-task.css'
})
export class AssignTask implements OnInit {
  @Input() taskId!: string;
  teamMembers: any[] = [];
  selectedUserId = '';
  

  constructor(private userService: UserService, private taskService: TaskService,private route: ActivatedRoute) {}

  ngOnInit(): void {
      this.taskId = this.route.snapshot.paramMap.get('id') || '';
      this.userService.getTeamMembers().subscribe({
        next : res =>{
            const users = res.data || [];
            this.teamMembers = users.filter((user: any)=> user.role === 'TeamMember');
        },
        error: err => console.log('failed to load team member', err)
      });
  }
  assign() {
    if (!this.selectedUserId) return;

    this.taskService.assignTask(this.taskId, this.selectedUserId).subscribe({
      next: () => alert('Task assigned!'),
      error: () => alert('Assignment failed')
    });
  }
}
