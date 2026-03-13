import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import {
  IonButton,
  IonCol,
  IonContent,
  IonGrid,
  IonIcon,
  IonInput,
  IonRow,
  IonSpinner,
  IonText,
  IonTitle,
} from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import { logoGoogle } from 'ionicons/icons';
import { AuthService } from '../../core/services/auth.service';
import { BrandMarkComponent } from '../../shared/components/brand-mark/brand-mark.component';

type LoginForm = FormGroup<{
  email: FormControl<string>;
  password: FormControl<string>;
}>;

@Component({
  selector: 'app-login',
  standalone: true,
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    BrandMarkComponent,
    IonButton,
    IonCol,
    IonContent,
    IonGrid,
    IonIcon,
    IonInput,
    IonRow,
    IonSpinner,
    IonText,
  ],
})
export class LoginComponent {
  isLoading = false;
  errorMessage = '';

  readonly loginForm: LoginForm = new FormGroup({
    email: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.email],
    }),
    password: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(6)],
    }),
  });

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router,
  ) {
    addIcons({ logoGoogle });
  }

  onSignIn(): void {
    if (this.loginForm.invalid || this.isLoading) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.errorMessage = '';
    this.isLoading = true;

    const email = this.loginForm.controls.email.value;
    const password = this.loginForm.controls.password.value;

    this.authService
      .login(email, password)
      .subscribe({
        next: async (isAuthenticated) => {
          this.isLoading = false;

          if (!isAuthenticated) {
            this.errorMessage = 'Invalid credentials. Please try again.';
            return;
          }

          await this.router.navigateByUrl('/home');
        },
        error: () => {
          this.isLoading = false;
          this.errorMessage = 'Sign-in failed. Please retry.';
        },
      });
  }

  onGoogleSignIn(): void {
    if (this.isLoading) {
      return;
    }

    this.loginForm.patchValue({
      email: 'google.user@habitoo.app',
      password: 'mock-password',
    });

    this.onSignIn();
  }
}
