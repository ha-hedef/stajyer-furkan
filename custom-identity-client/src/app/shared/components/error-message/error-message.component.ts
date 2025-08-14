import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-error-message',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div *ngIf="message" class="error-message">
      {{ message }}
    </div>
  `,
  styles: [`
    .error-message {
      background-color: #fff3f3;
      color: #dc3545;
      padding: 12px;
      border-radius: 4px;
      margin: 10px 0;
      border: 1px solid #ffcdd2;
    }
  `]
})
export class ErrorMessageComponent {
  @Input() message: string | null = null;
}