import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule,CommonModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.registerForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  register() {
    if (this.registerForm.invalid) {
      return;
    }

    this.isLoading = true;
    this.authService.register(this.registerForm.value)
      .subscribe({
        next: () => {
          this.isLoading = false;
          this.router.navigate(['/login']);
        },
        error: (err) => {
          this.isLoading = false;
          alert('Kayıt hatası: ' + err.error);
        }
      });
  }

  get f() {
    return this.registerForm.controls;
  }
}

import { Routes } from '@angular/router';
// import { LoginComponent } from './login/login.component';
// TODO: Update the path below to the correct location of LoginComponent
import { LoginComponent } from '../login/login.component';
import { CommonModule } from '@angular/common';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: '', redirectTo: 'login', pathMatch: 'full' }
];
