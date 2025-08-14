import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth/auth.service';


@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  username: string | null = '';

  constructor(private authService: AuthService) {}

  ngOnInit() {
    this.username = this.authService.getUsername();
  }
}
