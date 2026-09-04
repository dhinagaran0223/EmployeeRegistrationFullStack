import { Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { ToastComponent } from './services/toast.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, ToastComponent],
  template: `
    <header class="topbar">
      <div class="container nav">
        <a routerLink="/employees" class="brand">Employee Registration</a>
        <a routerLink="/employees/new" class="btn primary">+ Add Employee</a>
      </div>
    </header>
    <main class="container">
      <router-outlet />
    <app-toast />
    </main>
  `
})
export class AppComponent {}
