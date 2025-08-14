// filepath: c:\Users\kamis\OneDrive\Masaüstü\stajyer-furkan\custom-identity-client\src\app\dashboard\dashboard.component.ts
import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  username: string = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.username = this.authService.getUsername();
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}