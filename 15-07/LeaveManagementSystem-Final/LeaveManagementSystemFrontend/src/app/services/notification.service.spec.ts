import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { NotificationService, Notification } from './notification.service';
import { HttpHeaders } from '@angular/common/http';

describe('NotificationService', () => {
  let service: NotificationService;
  let httpMock: HttpTestingController;
  const baseUrl = 'http://localhost:5000/api/notifications';

  beforeEach(() => {
    // Setup fake token in localStorage
    localStorage.setItem('accessToken', 'fake-jwt-token');

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [NotificationService]
    });

    service = TestBed.inject(NotificationService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    localStorage.clear();
    httpMock.verify();
  });

  it('should fetch all notifications', () => {
    const userId = 'user123';
    const mockNotifications: { $values: Notification[] } = {
      $values: [
        {
          id: 'n1',
          message: 'Welcome!',
          createdAt: new Date().toISOString(),
          isRead: false,
          recipientId: userId
        }
      ]
    };

    service.getAll(userId).subscribe((res) => {
      expect(res.$values.length).toBe(1);
      expect(res.$values[0].message).toBe('Welcome!');
    });

    const req = httpMock.expectOne(`${baseUrl}/all/${userId}`);
    expect(req.request.method).toBe('GET');
    expect(req.request.headers.get('Authorization')).toBe('Bearer fake-jwt-token');
    req.flush(mockNotifications);
  });

  it('should fetch unread notifications', () => {
    const userId = 'user456';
    const mockResponse = {
      $values: [
        {
          id: 'n2',
          message: 'You have a task',
          createdAt: new Date().toISOString(),
          isRead: false,
          recipientId: userId
        }
      ]
    };

    service.getUnread(userId).subscribe((res) => {
      expect(res.$values.length).toBeGreaterThan(0);
      expect(res.$values[0].isRead).toBeFalse();
    });

    const req = httpMock.expectOne(`${baseUrl}/unread/${userId}`);
    expect(req.request.method).toBe('GET');
    expect(req.request.headers.get('Authorization')).toBe('Bearer fake-jwt-token');
    req.flush(mockResponse);
  });

    it('should mark a notification as read', () => {
    const notificationId = 'n3';

    service.markAsRead(notificationId).subscribe((res) => {
        expect(res).toBeNull(); // changed from undefined to null
    });

    const req = httpMock.expectOne(`${baseUrl}/mark-as-read/${notificationId}`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({});
    expect(req.request.headers.get('Authorization')).toBe('Bearer fake-jwt-token');
    req.flush(null); // returns null, not undefined
    });

    it('should mark all notifications as read for a user', () => {
    const userId = 'user789';

    service.markAllAsRead(userId).subscribe((res) => {
        expect(res).toBeNull(); // changed from undefined to null
    });

    const req = httpMock.expectOne(`${baseUrl}/mark-all-as-read/${userId}`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({});
    expect(req.request.headers.get('Authorization')).toBe('Bearer fake-jwt-token');
    req.flush(null);
    });

});
