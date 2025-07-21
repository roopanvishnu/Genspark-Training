import { TestBed } from '@angular/core/testing';
import { SignalRService } from './signal-r.service';
import * as signalR from '@microsoft/signalr';

fdescribe('SignalRService', () => {
  let service: SignalRService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [SignalRService]
    });

    service = TestBed.inject(SignalRService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should create a SignalR HubConnection with expected configuration', () => {
    const connection = service.createConnection();

    expect(connection).toBeTruthy();
    expect(connection).toEqual(jasmine.any(signalR.HubConnection));
    expect(typeof connection.start).toBe('function');
    expect(typeof connection.stop).toBe('function');
    expect(typeof connection.on).toBe('function');
    expect(typeof connection.invoke).toBe('function');
  });
});
