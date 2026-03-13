import { Injectable } from '@angular/core';
import { Preferences } from '@capacitor/preferences';
import { Observable, of } from 'rxjs';
import { delay, tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private static readonly AccessTokenKey = 'habitoo_access_token';

  login(email: string, password: string): Observable<boolean> {
    const isValidMockCredentials = Boolean(email && password);
    return of(isValidMockCredentials).pipe(
      delay(1000),
      tap((isAuthenticated) => {
        if (isAuthenticated) {
          void this.storeToken(`mock-session-${Date.now()}`);
        }
      }),
    );
  }

  async loginWithGoogle(): Promise<string> {
    // Mock Google auth handshake until backend auth flow is connected.
    const mockToken = `mock-google-token-${Date.now()}`;
    await this.storeToken(mockToken);
    return mockToken;
  }

  async storeToken(token: string): Promise<void> {
    await Preferences.set({
      key: AuthService.AccessTokenKey,
      value: token,
    });
  }

  async getToken(): Promise<string | null> {
    const { value } = await Preferences.get({
      key: AuthService.AccessTokenKey,
    });

    return value;
  }

  async clearToken(): Promise<void> {
    await Preferences.remove({
      key: AuthService.AccessTokenKey,
    });
  }

  async logout(): Promise<void> {
    await this.clearToken();
  }
}
