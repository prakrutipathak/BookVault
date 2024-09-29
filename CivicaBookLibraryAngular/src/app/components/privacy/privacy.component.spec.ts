import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FAQ, PrivacyComponent } from './privacy.component';

describe('PrivacyComponent', () => {
  let component: PrivacyComponent;
  let fixture: ComponentFixture<PrivacyComponent>;
  const faqs: FAQ[] = [
    {
      question: 'How do I issue a book?',
      answer: 'To issue a book, simply log in to your account, search for the book you want, and click on the \'issue\' button at book reports.',
      showAnswer: false
    },
    {
      question: 'How many books can I issue at once?',
      answer: 'You can issue up to 2 books simultaneously from our library.',
      showAnswer: false
    }
  ];
  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [PrivacyComponent]
    });
    fixture = TestBed.createComponent(PrivacyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should create', () => {
    component.toggleAnswer(faqs[1])
  });
});
