import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

import { environment } from '../../../environments/environment';
import { LoginRequest, LoginResponse } from '../models/auth.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
    private readonly http = inject(HttpClient);
    private readonly apiUrl = `${environment.apiUrl}/Auth`;
    private readonly tokenKey = 'ccl_inventory_token';

    login(credentials: LoginRequest): Observable<LoginResponse> {
        return this.http.post<LoginResponse>(`${this.apiUrl}/login`, credentials)
            .pipe(
                tap(response => {
                    sessionStorage.setItem(this.tokenKey, response.accessToken);
                })
            );
    }

    getToken(): string | null { return sessionStorage.getItem(this.tokenKey); }

    isAuthenticated(): boolean { return this.getToken() !== null; }

    logout(): void {
        sessionStorage.removeItem(this.tokenKey);
    }
}
