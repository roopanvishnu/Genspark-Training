import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { NgxChartsModule, LegendPosition } from '@swimlane/ngx-charts';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-dashboard',
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
  imports: [CommonModule, HttpClientModule, NgxChartsModule]
})
export class DashboardComponent implements OnInit {
  users: any[] = [];

  genderData: any[] = [];
  roleData: any[] = [];
  stateData: any[] = [];

  legendPosition = LegendPosition.Right;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.http.get<any>('https://dummyjson.com/users').subscribe(res => {
      this.users = res.users;
      this.prepareChartData();
    });
  }

  prepareChartData() {
    const genderCounts = this.countBy(this.users, 'gender');
    this.genderData = this.toChartData(genderCounts);

    const roleCounts = this.countBy(this.users, u => u.company?.title);
    this.roleData = this.toChartData(roleCounts);

    const stateCounts = this.countBy(this.users, u => u.address?.state);
    this.stateData = this.toChartData(stateCounts);
  }

  countBy(arr: any[], key: string | ((item: any) => string)) {
    const map = new Map<string, number>();
    for (let item of arr) {
      const val = typeof key === 'function' ? key(item) : item[key];
      if (val) map.set(val, (map.get(val) || 0) + 1);
    }
    return map;
  }

  toChartData(map: Map<string, number>): any[] {
    return Array.from(map.entries()).map(([name, value]) => ({ name, value }));
  }
}
