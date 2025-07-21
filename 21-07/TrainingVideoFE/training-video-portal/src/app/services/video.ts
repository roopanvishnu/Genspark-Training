// import { HttpClient } from '@angular/common/http';
// import { Injectable } from '@angular/core';
// import { Observable } from 'rxjs';
// import { TrainingVideo } from '../models/training-video.model';

// @Injectable({ providedIn: 'root' })
// export class VideoService {
//   private apiUrl = 'http://localhost:5213/api/videos';

//   constructor(private http: HttpClient) {}

//   getVideos(): Observable<TrainingVideo[]> {
//     return this.http.get<TrainingVideo[]>(this.apiUrl);
//   }

//   uploadVideo(formData: FormData): Observable<any> {
//     return this.http.post(`${this.apiUrl}/upload`, formData);
//   }
// }


import { HttpClient, HttpErrorResponse, HttpEventType } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, throwError, map } from 'rxjs';
import { TrainingVideo } from '../models/training-video.model';

@Injectable({ providedIn: 'root' })
export class VideoService {
  private apiUrl = 'http://localhost:5213/api/videos';

  constructor(private http: HttpClient) {}

  getVideos(): Observable<TrainingVideo[]> {
    return this.http.get<TrainingVideo[]>(this.apiUrl).pipe(
      catchError(this.handleError)
    );
  }

  uploadVideo(formData: FormData): Observable<any> {
    return this.http.post(`${this.apiUrl}/upload`, formData, {
      reportProgress: true,
      observe: 'events'
    }).pipe(
      map(event => {
        switch (event.type) {
          case HttpEventType.UploadProgress:
            const progress = Math.round(100 * event.loaded / (event.total || 1));
            return { status: 'progress', message: progress };
          case HttpEventType.Response:
            return { status: 'complete', body: event.body };
          default:
            return { status: 'uploading', message: 'Uploading...' };
        }
      }),
      catchError(this.handleError)
    );
  }

  // Alternative simpler upload method without progress tracking
  uploadVideoSimple(formData: FormData): Observable<any> {
    return this.http.post(`${this.apiUrl}/upload`, formData).pipe(
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'An unexpected error occurred';
    
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Server-side error
      switch (error.status) {
        case 400:
          errorMessage = 'Bad Request: Please check your input';
          break;
        case 413:
          errorMessage = 'File too large';
          break;
        case 415:
          errorMessage = 'Unsupported file type';
          break;
        case 500:
          errorMessage = 'Server error: Please try again later';
          break;
        case 0:
          errorMessage = 'Connection error: Please check your internet';
          break;
        default:
          errorMessage = `Server returned code: ${error.status}, error message: ${error.message}`;
      }
    }

    console.error('VideoService Error:', errorMessage);
    return throwError(() => error);
  }
}