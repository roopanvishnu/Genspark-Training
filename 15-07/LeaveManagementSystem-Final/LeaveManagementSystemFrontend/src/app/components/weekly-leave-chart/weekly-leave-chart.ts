// import { Component, OnInit } from '@angular/core';
// import { AgCharts } from 'ag-charts-angular';
// import { AgChartOptions } from 'ag-charts-community';
// import { LeaveService } from '../../services/leave.service';
// import { CommonModule } from '@angular/common';

// @Component({
//   selector: 'app-weekly-leave-chart',
//   standalone: true,
//   imports: [AgCharts, CommonModule],
//   templateUrl: './weekly-leave-chart.html',
// })
// export class WeeklyLeaveChart implements OnInit {
//   chartOptions!: AgChartOptions;

//   constructor(private leaveService: LeaveService) {}

//   ngOnInit(): void {
//     this.leaveService.getAllLeaveRequests(1, 1000, '', '', 'CreatedAt', 'desc').subscribe({
//       next: (res) => {
//         const requests = res.data?.data?.$values ?? [];

//         const currentMonth = new Date().getMonth();
//         const currentYear = new Date().getFullYear();

//         const weeklyCounts = [0, 0, 0, 0];

//         for (const leave of requests) {
//           const start = new Date(leave.startDate);
//           if (start.getMonth() === currentMonth && start.getFullYear() === currentYear) {
//             const weekNumber = Math.floor((start.getDate() - 1) / 7);
//             if (weekNumber >= 0 && weekNumber < 4) {
//               weeklyCounts[weekNumber]++;
//             }
//           }
//         }

//         const chartData = [
//           { week: 'Week 1', leaves: weeklyCounts[0] },
//           { week: 'Week 2', leaves: weeklyCounts[1] },
//           { week: 'Week 3', leaves: weeklyCounts[2] },
//           { week: 'Week 4', leaves: weeklyCounts[3] },
//         ];

//         this.chartOptions = {
//           data: chartData,
//           title: {
//             text: 'Weekly Leave Requests – Current Month',
//             fontSize: 18,
//             fontWeight: 'bold',
//           },
//           series: [
//             {
//               type: 'bar',
//               xKey: 'week',
//               yKey: 'leaves',
//               yName: 'Leave Count',
//               fill: '#4f46e5',
//               stroke: '#312e81',
//               cornerRadius: 6,
//               tooltip: { enabled: true },
//               label: {
//                 enabled: true,
//                 fontSize: 14,
//                 color: '#111827',
//               },
//             },
//           ],
//           axes: [
//             {
//               type: 'category',
//               position: 'bottom',
//               title: { text: 'Weeks' },
//               label: { fontSize: 14 },
//               gridLine: { enabled: false },
//             },
//             {
//               type: 'number',
//               position: 'left',
//               title: { text: 'Leave Count' },
//               label: { fontSize: 14 },
//               gridLine: { enabled: true },
//             },
//           ],
//           padding: {
//             top: 20,
//             right: 20,
//             bottom: 50,
//             left: 60,
//           },
//           legend: { enabled: false },
//           background: {
//             fill: '#ffffff',
//           },
//         };
//       },
//       error: (err) => {
//         console.error('Failed to load leave requests:', err);
//       },
//     });
//   }
// }
