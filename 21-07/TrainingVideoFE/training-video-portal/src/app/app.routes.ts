import { Routes } from '@angular/router';
import { VideoListComponent } from './components/video-list/video-list';
import { VideoUploadComponent } from './components/video-upload/video-upload';

export const routes: Routes = [
  { path: 'videos', component: VideoListComponent },
  { path: 'upload', component: VideoUploadComponent }
];
