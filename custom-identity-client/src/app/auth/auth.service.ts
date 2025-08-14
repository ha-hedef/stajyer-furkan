import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7214/api/auth';
  private usernameKey = 'username';
  private tokenKey = 'access_token';
  private refreshTokenKey = 'refresh_token';

  constructor(private http: HttpClient, private router: Router) {}

  login(credentials: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/login-step1`, credentials);
  }

  register(user: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, user);
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.refreshTokenKey);
    localStorage.removeItem(this.usernameKey);
    this.router.navigate(['/login']);
  }

  saveTokens(accessToken: string, refreshToken: string, username: string): void {
    localStorage.setItem(this.tokenKey, accessToken);
    localStorage.setItem(this.refreshTokenKey, refreshToken);
    localStorage.setItem(this.usernameKey, username);
  }

  getAccessToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.refreshTokenKey);
  }

  getUsername(): string {
    return localStorage.getItem(this.usernameKey) ?? '';
  }

  isLoggedIn(): boolean {
    const token = this.getAccessToken();
    if (!token) return false;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expiry = payload.exp * 1000;
      return Date.now() < expiry;
    } catch {
      return false;
    }
  }

  refreshToken(): Observable<any> {
    return this.http.post(`${this.apiUrl}/refresh-token`, {
      refreshToken: this.getRefreshToken()
    });
  }
}
