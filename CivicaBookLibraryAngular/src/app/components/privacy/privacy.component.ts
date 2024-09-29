import { Component } from '@angular/core';

export interface FAQ {
  question: string;
  answer: string;
  showAnswer: boolean;
}

@Component({
  selector: 'app-privacy',
  templateUrl: './privacy.component.html',
  styleUrls: ['./privacy.component.css']
})
export class PrivacyComponent {
  faqs: FAQ[] = [
    {
      question: 'How do I issue a book?',
      answer: 'To issue a book, simply log in to your account, search for the book you want, and click on the \'issue\' button at book reports.',
      showAnswer: false
    },
    {
      question: 'How many books can I issue at once?',
      answer: 'You can issue up to 2 books simultaneously from our library.',
      showAnswer: false
    },
    {
      question: 'How do I return a issue book?',
      answer: 'You can return a issue book online by logging into your account, going to your issued books, and clicking on the \'Return Book\' button next to the book you want to return.',
      showAnswer: false
    },
    {
      question: 'Can I issue books if I am not a member of the library?',
      answer: 'In most cases, you need to be a member of the library to issue books.',
      showAnswer: false
    },
    {
      question: 'How do I become a member of the library?',
      answer: 'To become a member, visit the library\'s website or location and fill out a registration form. You may need to provide your details.',
      showAnswer: false
    },
    {
      question: 'What information do I need to provide during registration?',
      answer: 'During registration, you typically need to provide your full name, email address, DOB, password, contact number and gender. ',
      showAnswer: false
    },
    {
      question: 'How do I log in to my library account?',
      answer: 'To log in, enter your registered loginId and password on the library app\'s login page. Click on the \'Login\' or \'Sign In\' button to access your account.',
      showAnswer: false
    },
    {
      question: 'What should I do if I forget my password?',
      answer: 'If you forget your password, you can usually click on the \'Forgot Password\' or \'Reset Password\' link on the login page. Follow the instructions to reset your password, which involve password hint and password hint answer you entered at registration time.',
      showAnswer: false
    },
    {
      question: 'Is my personal information secure?',
      answer: 'Yes, libraries take data security seriously and implement measures to protect your personal information. Ensure you use strong passwords and avoid sharing your account details with others.',
      showAnswer: false
    }
  ];

  toggleAnswer(faq: FAQ): void {
    faq.showAnswer = !faq.showAnswer;
  }
}
