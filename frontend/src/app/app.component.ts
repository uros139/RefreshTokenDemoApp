import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterOutlet } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, MatToolbarModule, MatIconModule, MatButtonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})

export class AppComponent {
  constructor(private router: Router) {
  }

  title = 'Students overview app';

  navigateToLogin() {
    debugger
    this.router.navigate(['/login']);
  }

  navigateToRegister() {
    debugger
    this.router.navigate(['/register']);
  }

  navigateToHome() {
    debugger
    this.router.navigate(['']);
  }
}
