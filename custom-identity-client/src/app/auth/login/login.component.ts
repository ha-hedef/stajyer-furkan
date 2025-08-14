import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { SpinnerComponent } from '../../shared/components/spinner/spinner.component';
import { ErrorMessageComponent } from '../../shared/components/error-message/error-message.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    CommonModule,
    SpinnerComponent,
    ErrorMessageComponent
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  isLoading = false;
  error: string | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]]
    });
  }

  login() {
    if (this.loginForm.invalid) {
      return;
    }

    this.isLoading = true;
    this.error = null;

    this.authService.login(this.loginForm.value)
      .subscribe({
        next: () => {
          this.router.navigate(['/dashboard']);
        },
        error: (err) => {
          this.isLoading = false;

          if (err.error instanceof ProgressEvent) {
            this.error = 'Sunucuya ulaşılamadı. Lütfen bağlantınızı kontrol edin.';
          } else if (typeof err.error === 'string') {
            this.error = err.error;
          } else if (err.error?.message) {
            this.error = err.error.message;
          } else {
            this.error = JSON.stringify(err.error);
          }
      }

    });

    this.isLoading = false;
  }

  get f() {
    return this.loginForm.controls;
  }
}
