
import { Component } from '@angular/core';
import { VideoService } from '../../services/video';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-video-upload',
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './video-upload.html',
  styleUrl: './video-upload.css'
})
export class VideoUploadComponent {
  title = '';
  description = '';
  selectedFile: File | null = null;
  selectedFileName = '';
  isUploading = false;
  uploadSuccess = false;
  uploadError = '';

  constructor(private videoService: VideoService) {}

  onFileChange(event: any) {
    const file = event.target.files[0];
    if (file) {
      // Validate file type
      if (!file.type.startsWith('video/')) {
        this.uploadError = 'Please select a valid video file';
        this.selectedFile = null;
        this.selectedFileName = '';
        this.clearErrorAfterDelay();
        return;
      }

      // Validate file size (e.g., max 500MB)
      const maxSize = 500 * 1024 * 1024; // 500MB
      if (file.size > maxSize) {
        this.uploadError = 'File size must be less than 500MB';
        this.selectedFile = null;
        this.selectedFileName = '';
        this.clearErrorAfterDelay();
        return;
      }

      this.selectedFile = file;
      this.selectedFileName = file.name;
      this.uploadError = '';
      this.uploadSuccess = false;
    }
  }

  onUpload(event?: Event) {
    if (event) {
      event.preventDefault();
    }

    // Validation
    if (!this.title.trim()) {
      this.uploadError = 'Please enter a video title';
      this.clearErrorAfterDelay();
      return;
    }

    if (!this.description.trim()) {
      this.uploadError = 'Please enter a video description';
      this.clearErrorAfterDelay();
      return;
    }

    if (!this.selectedFile) {
      this.uploadError = 'Please select a video file';
      this.clearErrorAfterDelay();
      return;
    }

    // Start upload process
    this.isUploading = true;
    this.uploadError = '';
    this.uploadSuccess = false;

    const formData = new FormData();
    formData.append('title', this.title.trim());
    formData.append('description', this.description.trim());
    formData.append('file', this.selectedFile);

    this.videoService.uploadVideo(formData).subscribe({
      next: (response) => {
        console.log('Upload successful:', response);
        this.uploadSuccess = true;
        this.uploadError = '';
        this.resetFormAfterSuccess();
      },
      error: (error) => {
        console.error('Upload error:', error);
        this.uploadSuccess = false;
        
        // Handle different error types
        if (error.status === 413) {
          this.uploadError = 'File too large. Please select a smaller video file.';
        } else if (error.status === 415) {
          this.uploadError = 'Unsupported file type. Please select a valid video file.';
        } else if (error.status === 400) {
          this.uploadError = error.error?.message || 'Invalid request. Please check your inputs.';
        } else if (error.status === 0) {
          this.uploadError = 'Connection error. Please check your internet connection.';
        } else {
          this.uploadError = error.error?.message || 'Upload failed. Please try again.';
        }
        
        this.clearErrorAfterDelay();
      },
      complete: () => {
        this.isUploading = false;
      }
    });
  }

  private resetFormAfterSuccess() {
    // Reset form after 3 seconds to show success message
    setTimeout(() => {
      this.title = '';
      this.description = '';
      this.selectedFile = null;
      this.selectedFileName = '';
      this.uploadSuccess = false;
      
      // Reset file input
      const fileInput = document.getElementById('video-file') as HTMLInputElement;
      if (fileInput) {
        fileInput.value = '';
      }
    }, 3000);
  }

  private clearErrorAfterDelay() {
    // Clear error message after 5 seconds
    setTimeout(() => {
      this.uploadError = '';
    }, 5000);
  }

  // Getter for form validation
  get isFormValid(): boolean {
    return !!(this.title.trim() && this.description.trim() && this.selectedFile);
  }

  // Method to format file size for display
  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  }

  // Get file size for display
  get fileSizeFormatted(): string {
    return this.selectedFile ? this.formatFileSize(this.selectedFile.size) : '';
  }
}