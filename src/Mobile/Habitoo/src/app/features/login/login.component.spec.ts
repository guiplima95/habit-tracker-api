import { fakeAsync, TestBed, tick } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { AuthService } from '../../core/services/auth.service';
import { LoginComponent } from './login.component';

describe('LoginComponent', () => {
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    authServiceSpy = jasmine.createSpyObj<AuthService>('AuthService', ['login']);
    routerSpy = jasmine.createSpyObj<Router>('Router', ['navigateByUrl']);
    routerSpy.navigateByUrl.and.resolveTo(true);

    await TestBed.configureTestingModule({
      imports: [LoginComponent],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: Router, useValue: routerSpy },
      ],
    }).compileComponents();
  });

  it('should create', () => {
    const fixture = TestBed.createComponent(LoginComponent);
    const component = fixture.componentInstance;
    expect(component).toBeTruthy();
  });

  it('should navigate to home after successful login', fakeAsync(() => {
    authServiceSpy.login.and.returnValue(of(true).pipe(delay(1000)));

    const fixture = TestBed.createComponent(LoginComponent);
    const component = fixture.componentInstance;

    component.loginForm.setValue({
      email: 'user@habitoo.app',
      password: 'strong-pass',
    });

    component.onSignIn();

    expect(component.isLoading).toBeTrue();

    tick(1000);

    expect(component.isLoading).toBeFalse();
    expect(routerSpy.navigateByUrl).toHaveBeenCalledWith('/home');
  }));

  it('should show an error when mocked login returns false', fakeAsync(() => {
    authServiceSpy.login.and.returnValue(of(false).pipe(delay(1000)));

    const fixture = TestBed.createComponent(LoginComponent);
    const component = fixture.componentInstance;

    component.loginForm.setValue({
      email: 'user@habitoo.app',
      password: 'strong-pass',
    });

    component.onSignIn();
    tick(1000);

    expect(component.errorMessage).toContain('Invalid credentials');
    expect(routerSpy.navigateByUrl).not.toHaveBeenCalled();
  }));
});
