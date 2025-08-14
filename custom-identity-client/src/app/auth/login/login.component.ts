// login.component.ts
import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject, takeUntil, timer } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit, OnDestroy {
  loginForm: FormGroup;
  isLoading = false;
  showPassword = false;
  errorMessage = '';
  successMessage = '';
  currentStep: 'credentials' | 'otp' = 'credentials';
  resendCooldown = 0;
  private destroy$ = new Subject<void>();

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private toastr: ToastrService
  ) {
    this.loginForm = this.initializeForm();
  }

  ngOnInit(): void {
    this.setupFormValidation();
    this.checkAuthState();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private initializeForm(): FormGroup {
    return this.formBuilder.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      otp: ['', [Validators.pattern(/^\d{6}$/)]]
    });
  }

  private setupFormValidation(): void {
    // Real-time validation
    this.loginForm.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.clearMessages();
      });

    // Add OTP validation when step changes
    this.loginForm.get('otp')?.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(value => {
        if (this.currentStep === 'otp' && value && value.length === 6) {
          this.onSubmit();
        }
      });
  }

  private checkAuthState(): void {
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/dashboard']);
    }
  }

  async onSubmit(): Promise<void> {
    if (this.loginForm.invalid || this.isLoading) {
      this.markFormGroupTouched();
      return;
    }

    this.isLoading = true;
    this.clearMessages();

    try {
      if (this.currentStep === 'credentials') {
        await this.handleCredentialsStep();
      } else {
        await this.handleOtpStep();
      }
    } catch (error: any) {
      this.handleError(error);
    } finally {
      this.isLoading = false;
    }
  }

  private async handleCredentialsStep(): Promise<void> {
    const { username, password } = this.loginForm.value;
    
    const response = await this.authService.loginStep1({
      username: username.trim(),
      password
    }).toPromise();

    if (response?.success) {
      this.currentStep = 'otp';
      this.addOtpValidation();
      this.successMessage = 'OTP sent to your email. Please check your inbox.';
      this.startResendCooldown();
      this.toastr.success('OTP sent to your email', 'Success');
    }
  }

  private async handleOtpStep(): Promise<void> {
    const { username, otp } = this.loginForm.value;
    
    const response = await this.authService.loginStep2({
      username: username.trim(),
      otp: otp.trim(),
      code: otp.trim() // Backend expects both otp and code
    }).toPromise();

    if (response?.success && response.data?.accessToken) {
      this.authService.setTokens(
        response.data.accessToken,
        response.data.refreshToken
      );
      
      this.successMessage = 'Login successful! Redirecting...';
      this.toastr.success('Welcome back!', 'Login Successful');
      
      // Redirect after short delay
      setTimeout(() => {
        this.router.navigate(['/dashboard']);
      }, 1500);
    }
  }

  private addOtpValidation(): void {
    this.loginForm.get('otp')?.setValidators([
      Validators.required,
      Validators.pattern(/^\d{6}$/)
    ]);
    this.loginForm.get('otp')?.updateValueAndValidity();
  }

  private startResendCooldown(): void {
    this.resendCooldown = 60;
    timer(0, 1000)
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        if (this.resendCooldown > 0) {
          this.resendCooldown--;
        }
      });
  }

  async resendOtp(): Promise<void> {
    if (this.resendCooldown > 0 || this.isLoading) return;

    this.isLoading = true;
    try {
      const username = this.loginForm.get('username')?.value;
      
      const response = await this.authService.sendOtp({
        username: username.trim()
      }).toPromise();

      if (response?.success) {
        this.successMessage = 'New OTP sent to your email.';
        this.startResendCooldown();
        this.toastr.success('New OTP sent', 'Success');
      }
    } catch (error: any) {
      this.handleError(error);
    } finally {
      this.isLoading = false;
    }
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  // Social login methods
  async loginWithGoogle(): Promise<void> {
    try {
      this.isLoading = true;
      // Implement Google OAuth login
      this.toastr.info('Google login coming soon', 'Info');
    } catch (error: any) {
      this.handleError(error);
    } finally {
      this.isLoading = false;
    }
  }

  async loginWithMicrosoft(): Promise<void> {
    try {
      this.isLoading = true;
      // Implement Microsoft OAuth login
      this.toastr.info('Microsoft login coming soon', 'Info');
    } catch (error: any) {
      this.handleError(error);
    } finally {
      this.isLoading = false;
    }
  }

  // Utility methods
  isFieldInvalid(fieldName: string): boolean {
    const field = this.loginForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  isFieldValid(fieldName: string): boolean {
    const field = this.loginForm.get(fieldName);
    return !!(field && field.valid && (field.dirty || field.touched));
  }

  private markFormGroupTouched(): void {
    Object.keys(this.loginForm.controls).forEach(key => {
      const control = this.loginForm.get(key);
      control?.markAsTouched();
    });
  }

  private clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  private handleError(error: any): void {
    console.error('Login error:', error);
    
    if (error?.error?.message) {
      this.errorMessage = error.error.message;
    } else if (error?.message) {
      this.errorMessage = error.message;
    } else {
      this.errorMessage = 'An unexpected error occurred. Please try again.';
    }

    this.toastr.error(this.errorMessage, 'Login Failed');
    
    // Reset to credentials step if OTP fails
    if (this.currentStep === 'otp' && error?.status === 401) {
      this.currentStep = 'credentials';
      this.loginForm.get('otp')?.reset();
      this.loginForm.get('otp')?.clearValidators();
      this.loginForm.get('otp')?.updateValueAndValidity();
    }
  }

  // Keyboard navigation
  onKeyDown(event: KeyboardEvent): void {
    if (event.key === 'Enter') {
      event.preventDefault();
      this.onSubmit();
    }
  }

  // Focus management for accessibility
  focusNextField(currentField: string): void {
    const fields = ['username', 'password', 'otp'];
    const currentIndex = fields.indexOf(currentField);
    
    if (currentIndex >= 0 && currentIndex < fields.length - 1) {
      const nextField = fields[currentIndex + 1];
      const element = document.getElementById(nextField);
      element?.focus();
    }
  }
}