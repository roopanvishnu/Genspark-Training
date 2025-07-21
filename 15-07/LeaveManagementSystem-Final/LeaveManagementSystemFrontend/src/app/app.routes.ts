import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { Dashboard } from './pages/dashboard/dashboard';
import { ApplyLeave } from './pages/apply-leave/apply-leave';
import { LeaveApprovals } from './pages/leave-approvals/leave-approvals';
import { LeaveHistoryById } from './pages/leave-history-by-id/leave-history-by-id';
import { LeaveBalance } from './pages/leave-balance/leave-balance';

//  Import Guards
import { authGuard } from './guards/auth-guard'
import { hrGuard } from './guards/hr-guard';
import { UserManagement } from './pages/user-management/user-management';
import { UserAdd } from './pages/user-management/user-add/user-add';
import { UserEdit } from './pages/user-management/user-edit/user-edit';
// import { LeaveDetails } from './pages/leave-details/leave-details';
import { UserProfile } from './pages/user-profile/user-profile';
import { LeaveTypes } from './pages/leave-types/leave-types';
import { NotificationComponent } from './pages/notification/notification';
import { UserLeaveBalanceComponent } from './pages/user-leave-balance/user-leave-balance';
import { LeaveCalendarComponent } from './components/leave-calendar/leave-calendar';
import { LeaveDetails } from './pages/leave-details/leave-details';
import { LeaveHistory } from './pages/leave-history/leave-history';
import { UserDetailComponent } from './pages/user-management/user-detail/user-detail';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: Login },

  // Protected routes
  { path: 'dashboard', component: Dashboard, canActivate: [authGuard] },
  { path: 'apply-leave', component: ApplyLeave, canActivate: [authGuard] },
  { path: 'leave-history', component: LeaveHistory, canActivate: [authGuard] },
  { path: 'leave-history/:id', component: LeaveHistoryById, canActivate: [authGuard] },
  { path: 'leave-balance', component: LeaveBalance, canActivate: [authGuard] },
  { path: 'notifications', component: NotificationComponent, canActivate: [authGuard] },

  // HR-only route
  { path: 'leave-approvals', component: LeaveApprovals, canActivate: [authGuard, hrGuard] },
  { path: 'user-management', component: UserManagement, canActivate: [authGuard, hrGuard] },

  { path: 'users/add', component: UserAdd, canActivate: [authGuard, hrGuard] },
  { path: 'users/edit/:id', component: UserEdit, canActivate: [authGuard, hrGuard] },
  { path: 'leave-approvals/:id', component: LeaveDetails },
  { path: 'profile', component: UserProfile, canActivate: [authGuard] },
  { path: 'leave-types', component: LeaveTypes},
  { path: 'my-leave-balances',component: UserLeaveBalanceComponent},
  { path: 'my-calendar',component: LeaveCalendarComponent},
  { path: 'users/:id/detail', component: UserDetailComponent }
];
