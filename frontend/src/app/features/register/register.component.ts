import { Component } from '@angular/core';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { AbstractControl, AbstractControlOptions, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { Router } from '@angular/router';
import { AuthenticationClient, RegistrationModel } from '../../../generated/api/api-reference';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [MatCardModule, MatInputModule, FormsModule, HttpClientModule, ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
  providers: [AuthenticationClient]
})
export class RegisterComponent {
  registrationForm: FormGroup;
  errorMessage: string = '';

  constructor(private formBuilder: FormBuilder, private router: Router, private authClient: AuthenticationClient) {
    const formOptions: AbstractControlOptions = {
      validators: this.passwordMatchValidator()
    };

    this.registrationForm = this.formBuilder.group({
      username: ['', Validators.required],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    }, formOptions);
  }

  passwordMatchValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
      const password = control.get('password');
      const confirmPassword = control.get('confirmPassword');

      if (password && confirmPassword && password.value !== confirmPassword.value) {
        confirmPassword.setErrors({ passwordMismatch: true });
        return { passwordMismatch: true };
      } else {
        confirmPassword!.setErrors(null);
        return null;
      }
    };
  }

  register() {
    if (this.registrationForm.invalid) {
      return;
    }

    this.authClient.register(new RegistrationModel({
      username: this.registrationForm.value.username,
      firstName: this.registrationForm.value.firstName,
      lastName: this.registrationForm.value.lastName,
      email: this.registrationForm.value.email,
      password: this.registrationForm.value.password

    })).subscribe(() => {
      this.router.navigate(['/students-list'])
    },
      (error) => {
        this.errorMessage = error.message; // Assuming the error object has a 'message' property
      }
    )
  }
}