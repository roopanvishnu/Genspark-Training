export interface Notification {
  id: string;
  message: string;
  createdAt: string;
  isRead: boolean;
  readAt?: string;
  recipientId: string;
}