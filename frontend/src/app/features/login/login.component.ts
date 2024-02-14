import { Component } from '@angular/core';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { FormsModule } from '@angular/forms';
import { AuthenticationClient, LoginModel } from '../../../generated/api/api-reference';
import { HttpClientModule } from '@angular/common/http'; 

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [MatCardModule, MatInputModule, FormsModule, HttpClientModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  providers: [AuthenticationClient]
})
export class LoginComponent {

  constructor(private authClient: AuthenticationClient) {
  }

  username: string = '';
  password: string = '';

  login() {
    debugger;
    this.authClient.login(new LoginModel({
      username: this.username,
      password: this.password
    })).subscribe({
      next: () => {
        debugger
        console.log('Login successful');
      },
      error: (error) => {
        debugger
        console.error('Login failed:', error);
      }
    });

    console.log('Username:', this.username);
    console.log('Password:', this.password);
  }
}
