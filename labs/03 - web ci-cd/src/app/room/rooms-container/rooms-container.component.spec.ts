import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RoomsContainerComponent } from './rooms-container.component';

describe('RoomsContainerComponent', () => {
  let component: RoomsContainerComponent;
  let fixture: ComponentFixture<RoomsContainerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RoomsContainerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RoomsContainerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
