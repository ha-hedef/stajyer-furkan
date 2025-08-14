import { Component, Input } from '@angular/core';
import { NgStyle } from '@angular/common';
@Component({
  selector: 'app-spinner',
  standalone: true,
  imports: [NgStyle],
  template: `
    <div class="spinner" [ngStyle]="{'width': size+'px', 'height': size+'px'}">
      <div class="spinner-border" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
  `,
  styles: [`
    .spinner {
      display: flex;
      justify-content: center;
      align-items: center;
    }
    .spinner-border {
      width: 100%;
      height: 100%;
      border: 3px solid currentColor;
      border-right-color: transparent;
      border-radius: 50%;
      animation: spin 1s linear infinite;
    }
    @keyframes spin {
      to { transform: rotate(360deg); }
    }
    .visually-hidden {
      position: absolute;
      width: 1px;
      height: 1px;
      padding: 0;
      margin: -1px;
      overflow: hidden;
      clip: rect(0,0,0,0);
      border: 0;
    }
  `]
})
export class SpinnerComponent {
  @Input() size: number = 24;
}