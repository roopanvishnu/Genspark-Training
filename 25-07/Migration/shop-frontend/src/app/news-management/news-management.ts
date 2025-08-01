import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NewsService, NewsItem, NewsAddDTO } from '../services/news.service';

@Component({
  selector: 'app-news-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './news-management.html',
  styleUrls: ['./news-management.css']
})
export class NewsManagement implements OnInit {
  news: NewsItem[] = [];
  form: NewsAddDTO = {
    title: '',
    shortDescription: '',
    content: '',
    image: '',
    createdDate: '',
    status: 1,
    userId: 0
  };
  editingId: number | null = null;

  constructor(private newsService: NewsService) {}

  ngOnInit(): void {
    this.loadNews();
  }

  loadNews() {
    this.newsService.getAll().subscribe(res => this.news = res);
  }

  save() {
    const payload = {
      ...this.form,
      createdDate: new Date().toISOString(),
      userId: 1 // Replace with actual logged-in user ID from token if needed
    };

    if (this.editingId) {
      this.newsService.update(this.editingId, payload).subscribe(() => {
        this.cancel();
        this.loadNews();
      });
    } else {
      this.newsService.create(payload).subscribe(() => {
        this.cancel();
        this.loadNews();
      });
    }
  }

  edit(news: NewsItem) {
    this.editingId = news.newsId;
    this.form = {
      title: news.title,
      shortDescription: news.shortDescription,
      content: news.content,
      image: news.image,
      createdDate: news.createdDate,
      status: news.status,
      userId: news.userId
    };
  }

  delete(id: number) {
    this.newsService.delete(id).subscribe(() => this.loadNews());
  }

  cancel() {
    this.editingId = null;
    this.form = {
      title: '',
      shortDescription: '',
      content: '',
      image: '',
      createdDate: '',
      status: 1,
      userId: 0
    };
  }

  exportCSV() {
    this.newsService.exportCSV().subscribe(blob => {
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = 'news.csv';
      link.click();
      window.URL.revokeObjectURL(url);
    });
  }
}
