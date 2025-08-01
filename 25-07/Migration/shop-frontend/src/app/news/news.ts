import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NewsService, NewsItem } from '../services/news.service';

@Component({
  selector: 'app-news',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './news.html',
  styleUrls: ['./news.css']
})
export class News implements OnInit {
  news: NewsItem[] = [];

  constructor(private newsService: NewsService) {}

  ngOnInit(): void {
    this.newsService.getPublicNews().subscribe(res => this.news = res);
  }
}
