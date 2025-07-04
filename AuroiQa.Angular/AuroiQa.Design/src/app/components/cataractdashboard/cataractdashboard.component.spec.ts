import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CataractdashboardComponent } from './cataractdashboard.component';

describe('CataractdashboardComponent', () => {
  let component: CataractdashboardComponent;
  let fixture: ComponentFixture<CataractdashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CataractdashboardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CataractdashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
