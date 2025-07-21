import { Component, OnInit } from '@angular/core';
import { VideoService } from '../../services/video';
import { TrainingVideo } from '../../models/training-video.model';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-video-list',
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './video-list.html',
  styleUrl: './video-list.css'
})
export class VideoListComponent implements OnInit {
  videos: TrainingVideo[] = [];

  constructor(private videoService: VideoService) {}

  ngOnInit() {
    this.videoService.getVideos().subscribe(v => this.videos = v);
  }
}
