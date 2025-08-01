import { TestBed } from '@angular/core/testing';

import { Contactus } from './contactus';

describe('Contactus', () => {
  let service: Contactus;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Contactus);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
