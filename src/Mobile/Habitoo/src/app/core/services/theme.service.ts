import { Injectable, signal } from '@angular/core';
import { Preferences } from '@capacitor/preferences';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private static readonly ThemePreferenceKey = 'habitoo_theme_preference';
  private readonly darkModeState = signal(false);

  readonly isDarkMode = this.darkModeState.asReadonly();

  async initialize(): Promise<void> {
    const { value } = await Preferences.get({
      key: ThemeService.ThemePreferenceKey,
    });

    const shouldUseDarkMode = value === 'dark';
    this.applyTheme(shouldUseDarkMode);
  }

  async toggleTheme(): Promise<void> {
    const nextValue = !this.darkModeState();
    this.applyTheme(nextValue);

    await Preferences.set({
      key: ThemeService.ThemePreferenceKey,
      value: nextValue ? 'dark' : 'light',
    });
  }

  private applyTheme(isDarkMode: boolean): void {
    this.darkModeState.set(isDarkMode);
    document.body.classList.toggle('theme-dark', isDarkMode);
  }
}
