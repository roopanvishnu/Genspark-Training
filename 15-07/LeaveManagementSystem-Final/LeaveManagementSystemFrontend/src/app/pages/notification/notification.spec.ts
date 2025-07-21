import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { NotificationComponent } from './notification';
import { NotificationService } from '../../services/notification.service';
import { of } from 'rxjs';

const mockUnreadNotifications = {
  $values: [
    {
      id: '1',
      message: 'Unread Message 1',
      createdAt: new Date().toISOString(),
      isRead: false,
      recipientId: 'test-user'
    },
    {
      id: '2',
      message: 'Unread Message 2',
      createdAt: new Date().toISOString(),
      isRead: false,
      recipientId: 'test-user'
    }
  ]
};

const mockReadNotifications = {
  $values: [
    {
      id: '3',
      message: 'Read Message 1',
      createdAt: new Date().toISOString(),
      isRead: true,
      recipientId: 'test-user'
    }
  ]
};

describe('NotificationComponent', () => {
  let component: NotificationComponent;
  let fixture: ComponentFixture<NotificationComponent>;
  let notificationServiceSpy: jasmine.SpyObj<NotificationService>;

  beforeEach(async () => {
    const notifSpy = jasmine.createSpyObj('NotificationService', ['getAll', 'getUnread', 'markAsRead']);

    await TestBed.configureTestingModule({
      imports: [NotificationComponent], // standalone
      providers: [
        { provide: NotificationService, useValue: notifSpy }
      ]
    }).compileComponents();

    localStorage.setItem('userId', 'test-user');

    fixture = TestBed.createComponent(NotificationComponent);
    component = fixture.componentInstance;
    notificationServiceSpy = TestBed.inject(NotificationService) as jasmine.SpyObj<NotificationService>;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load unread notifications on init', fakeAsync(() => {
    notificationServiceSpy.getUnread.and.returnValue(of(mockUnreadNotifications));

    fixture.detectChanges();
    tick();

    expect(notificationServiceSpy.getUnread).toHaveBeenCalledWith('test-user');
    expect(component.unreadNotifications.length).toBe(2);
    expect(component.unreadCount).toBe(2);
  }));

  it('should switch to read tab and fetch read notifications', fakeAsync(() => {
    notificationServiceSpy.getAll.and.returnValue(of(mockReadNotifications));

    component.switchTab('read');
    tick();

    expect(notificationServiceSpy.getAll).toHaveBeenCalledWith('test-user');
    expect(component.readNotifications.length).toBe(1);
  }));

 
  it('should load more notifications on scroll', fakeAsync(() => {
    component.currentTab = 'unread';
    notificationServiceSpy.getUnread.and.returnValue(of(mockUnreadNotifications));
    component.unreadNotifications = [...mockUnreadNotifications.$values];

    component.onScroll({
      target: {
        scrollHeight: 1000,
        scrollTop: 900,
        clientHeight: 100
      }
    } as unknown as Event);

    tick();
    expect(component.page).toBe(2);
    expect(notificationServiceSpy.getUnread).toHaveBeenCalledTimes(1);
  }));
});
