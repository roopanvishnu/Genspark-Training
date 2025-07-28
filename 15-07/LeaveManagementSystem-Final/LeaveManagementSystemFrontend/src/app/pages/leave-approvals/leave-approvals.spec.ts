// import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
// import { LeaveApprovals } from './leave-approvals';
// import { LeaveRequestService } from '../../services/leave-request.service';
// import { of, throwError } from 'rxjs';
// import { LeaveRequestResponse } from '../../models/leave-request-response.model';
// import { CommonModule } from '@angular/common';
// import { RouterModule } from '@angular/router';
// import { FormsModule } from '@angular/forms';

// describe('LeaveApprovals Component', () => {
//   let component: LeaveApprovals;
//   let fixture: ComponentFixture<LeaveApprovals>;
//   let leaveRequestServiceSpy: jasmine.SpyObj<LeaveRequestService>;

//   const mockLeaveRequests: LeaveRequestResponse[] = [
//     {
//       id: '1',
//       userName: 'Alice',
//       reason: 'Vacation',
//       leaveTypeName: 'Annual Leave',
//       status: 'Pending',
//       createdAt: new Date().toISOString(),
//       updatedAt: new Date().toISOString(),
//       startDate: new Date().toISOString(),
//       endDate: new Date().toISOString(),
//       reviewedAt: '',
//       reviewComments: undefined,
//       userId: '',
//       leaveTypeId: ''
//     },
//     {
//       id: '2',
//       userName: 'Bob',
//       reason: 'Sick leave',
//       leaveTypeName: 'Sick',
//       status: 'Approved',
//       createdAt: new Date().toISOString(),
//       updatedAt: new Date().toISOString(),
//       startDate: new Date().toISOString(),
//       endDate: new Date().toISOString(),
//       reviewedAt: '',
//       reviewComments: undefined,
//       userId: '',
//       leaveTypeId: ''
//     }
//   ];

//   beforeEach(waitForAsync(() => {
//     const spy = jasmine.createSpyObj('LeaveRequestService', ['getAllLeaveRequests']);

//     TestBed.configureTestingModule({
//       imports: [CommonModule, FormsModule, RouterModule],
//       declarations: [],
//       providers: [{ provide: LeaveRequestService, useValue: spy }]
//     }).compileComponents();

//     leaveRequestServiceSpy = TestBed.inject(LeaveRequestService) as jasmine.SpyObj<LeaveRequestService>;
//     fixture = TestBed.createComponent(LeaveApprovals);
//     component = fixture.componentInstance;
//   }));

//   it('should create the component', () => {
//     expect(component).toBeTruthy();
//   });

//   it('should fetch and filter leave requests on init', () => {
//     leaveRequestServiceSpy.getAllLeaveRequests.and.returnValue(of({ data: { $values: mockLeaveRequests } }));

//     component.ngOnInit();

//     expect(leaveRequestServiceSpy.getAllLeaveRequests).toHaveBeenCalled();
//     expect(component.leaveRequests.length).toBe(2);
//     expect(component.filteredRequests.length).toBeGreaterThan(0);
//   });

//   it('should handle fetch error', () => {
//     leaveRequestServiceSpy.getAllLeaveRequests.and.returnValue(throwError(() => new Error('API Error')));

//     component.ngOnInit();

//     expect(component.errorMsg).toBe('Error loading leave approvals');
//   });

//   it('should search and filter leave requests by search term and status', () => {
//     leaveRequestServiceSpy.getAllLeaveRequests.and.returnValue(of({ data: { $values: mockLeaveRequests } }));
//     component.ngOnInit();

//     component.searchTerm = 'Alice';
//     component.statusFilter = 'Pending';
//     component.onSearchChange();

//     expect(component.filteredRequests.length).toBe(1);
//     expect(component.filteredRequests[0].userName).toBe('Alice');
//   });

//   it('should paginate correctly', () => {
//     leaveRequestServiceSpy.getAllLeaveRequests.and.returnValue(of({ data: { $values: [...mockLeaveRequests, ...mockLeaveRequests] } }));
//     component.pageSize = 1;
//     component.ngOnInit();

//     const firstPageItem = component.filteredRequests[0];
//     component.nextPage();
//     const secondPageItem = component.filteredRequests[0];

//     expect(component.page).toBe(2);
//     expect(secondPageItem).not.toBe(firstPageItem);
//   });

//   it('should track by leave id', () => {
//     const id = component.trackByLeaveId(0, mockLeaveRequests[0]);
//     expect(id).toBe('1');
//   });
// });
