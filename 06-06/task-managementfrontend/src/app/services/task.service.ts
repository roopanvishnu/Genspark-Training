import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class TaskService {
    private apiUrl = 'https://localhost:7120/api/v1/tasks';
    private readonly api = 'https://localhost:7120/api/v1/tasks';
    private readonly deleteapi = 'https://localhost:7120/api/v1/tasks/deleted'

    constructor(private http: HttpClient) { }

    getAllTasks(page = 1, limit = 10): Observable<any> {
        return this.http.get(`${this.apiUrl}?page=${page}&limit=${limit}`);
    }


    createTask(formData: FormData): Observable<any> {
        return this.http.post(`${this.apiUrl}`, formData); 
    }

    getTaskById(taskId: string): Observable<any> {
        return this.http.get(`${this.apiUrl}/${taskId}`);
    }

    getDeletedTasks() {
        return this.http.get<any>(`${this.deleteapi}`);
    }



    updateTask(id: string, formData: FormData): Observable<any> {
        return this.http.put(`${this.apiUrl}/${id}`, formData);
    }
    deleteTask(taskId: string) {
        return this.http.delete(`${this.apiUrl}/${taskId}`);
    }


    broadcastTask(taskId: string): Observable<any> {
        return this.http.post(`${this.apiUrl}/${taskId}/broadcast`, {});
    }

    getTaskAttachment(taskId: string) {
        const url = `https://localhost:7120/api/v1/tasks/${taskId}/download`;
        return this.http.get(url, { responseType: 'blob', observe: 'response' });
    }

    assignTask(taskId: string, userId: string) {
        return this.http.post(`https://localhost:7120/api/v1/tasks/${taskId}/assign/${userId}`, {});
    }

    getAssignedTasks(): Observable<any> {
        return this.http.get(`${this.apiUrl}/assigned`);
    }

    downloadAttachment(taskId: string): Observable<Blob> {
        const url = `${this.api}/${taskId}/download`;
        return this.http.get(url, { responseType: 'blob' });
    }
    updateTaskStatus(taskId: string, payload: { status: string, comment?: string }) {
        return this.http.put(`https://localhost:7120/api/v1/tasks/${taskId}/update-status`, payload);
    }


}
