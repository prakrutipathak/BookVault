import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SignupsuccessComponent } from './signupsuccess.component';

describe('SignupsuccessComponent', () => {
  let component: SignupsuccessComponent;
  let fixture: ComponentFixture<SignupsuccessComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [SignupsuccessComponent]
    });
    fixture = TestBed.createComponent(SignupsuccessComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
