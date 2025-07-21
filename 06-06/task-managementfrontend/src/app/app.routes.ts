import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { Login } from './auth/login/login';
import { Register } from './auth/register/register';
import { AuthGuard } from './guards/auth.gaurd';
import { RoleGuard } from './guards/role.guard';
import { TaskForm } from './pages/task-form/task-form';
import { TaskListManager } from './pages/task-list-manager/task-list-manager';
import { AssignTask } from './pages/assign-task/assign-task';
import { Notifications } from './pages/notifications/notifications';


export const routes: Routes = [
    {
        path: '',
        component: Login,
        pathMatch: 'full' // Ensures exact match for root path
    },
    {
        path: 'home',
        component: Home,
        pathMatch: 'full',
    },
    {
        path: 'login',
        component: Login
    },
    {
        path: 'register',
        component: Register
    },
    {
        path: 'task-form',
        component: TaskForm
    },
    {
        path: 'manager/tasks',
        component: TaskListManager
    },
    {
        path: 'manager/tasks/:id/edit',
        component: TaskForm
    },
    {
        path: 'manager/tasks/:id/assign',
        component: AssignTask
    },
    {
        path: 'team/tasks',
        canActivate: [AuthGuard],
        loadComponent: () => import('./pages/team-dashboard/team-dashboard').then(m => m.TeamDashboard),
    },
    {
        path: 'notifications',
        component: Notifications
    },
    {
        path: 'manager/tasks/deleted',
        loadComponent: () => import('./pages/task-list-deleted/task-list-deleted')
            .then(m => m.TaskListDeleted)
    }
];

// export const routes: Routes = [
//     {
//         path: 'home',
//         component: Home,
//         pathMatch: 'full',
//     },
//     {
//         path: 'login',
//         component: Login
//     },
//     {
//         path: 'register',
//         component: Register
//     },
    
//     {
//         path: 'task-form',
//         component: TaskForm
//     },
//     {
//         path: 'manager/tasks',
//         component: TaskListManager
//     },
//     {
//         path: 'manager/tasks/:id/edit',
//         component: TaskForm
//     },
//     {
//         path: 'manager/tasks/:id/assign',
//         component: AssignTask
//     },
//     {
//         path: 'team/tasks',
//         canActivate: [AuthGuard],
//         loadComponent: () => import('./pages/team-dashboard/team-dashboard').then(m => m.TeamDashboard),
//     },
//     {   path: 'notifications', 
//         component: Notifications 
//     },
//     {
//   path: 'manager/tasks/deleted',
//   loadComponent: () => import('./pages/task-list-deleted/task-list-deleted')
//     .then(m => m.TaskListDeleted)
// }

// ];
