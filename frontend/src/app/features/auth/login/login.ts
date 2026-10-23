import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { AuthService } from '../../../core/services/auth.service';

@Component({ selector: 'app-login', standalone: true, imports: [CommonModule, ReactiveFormsModule], templateUrl: './login.html', styleUrl: './login.scss' })
export class Login {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  loading = false;
  errorMessage = '';

  readonly loginForm = this.fb.nonNullable.group({
    username: ['', [Validators.required]],
    password: ['', [Validators.required]]
  });

  onSubmit(): void {
    if (this.loginForm.invalid || this.loading) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    this.authService.login(this.loginForm.getRawValue()).subscribe({
      next: () => {
        this.loading = false;
        void this.router.navigateByUrl('/dashboard');
      },
      error: (error) => {
        this.loading = false;

        if (error.status === 0) {
          this.errorMessage = 'No fue posible conectar con el servidor. Verifica que la API esté activa.';
        } else if (error.status === 401) {
          this.errorMessage = 'Usuario o contraseña incorrectos.';
        } else {
          this.errorMessage = 'No fue posible iniciar sesión. Intenta nuevamente.';
        }
      }
    });
  }
}