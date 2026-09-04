import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ToastService } from './toast.service';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule],
  template: `<div class="toast" *ngIf="toast.message()">{{ toast.message() }}</div>`
})
export class ToastComponent {
  toast = inject(ToastService);
}
