import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NewsManagement } from './news-management';

describe('NewsManagement', () => {
  let component: NewsManagement;
  let fixture: ComponentFixture<NewsManagement>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NewsManagement]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NewsManagement);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
