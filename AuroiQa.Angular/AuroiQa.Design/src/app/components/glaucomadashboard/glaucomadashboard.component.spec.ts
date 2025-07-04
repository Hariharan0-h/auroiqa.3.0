import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GlaucomadashboardComponent } from './glaucomadashboard.component';

describe('GlaucomadashboardComponent', () => {
  let component: GlaucomadashboardComponent;
  let fixture: ComponentFixture<GlaucomadashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GlaucomadashboardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GlaucomadashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
