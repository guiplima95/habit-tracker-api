import { AfterViewInit, Component, DestroyRef, ElementRef, OnDestroy, OnInit, ViewChild, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router, RouterLink } from '@angular/router';
import {
  IonBackButton,
  IonButton,
  IonButtons,
  IonCard,
  IonCardContent,
  IonCardHeader,
  IonCardTitle,
  IonContent,
  IonHeader,
  IonIcon,
  IonText,
  IonTitle,
  IonToolbar,
} from '@ionic/angular/standalone';
import { Chart } from 'chart.js/auto';
import { MatrixController, MatrixElement } from 'chartjs-chart-matrix';
import { addIcons } from 'ionicons';
import { logOutOutline, moonOutline, sunnyOutline } from 'ionicons/icons';
import { AuthService } from '../../core/services/auth.service';
import { ThemeService } from '../../core/services/theme.service';
import { Habit } from '../../shared/models/habit.model';
import { HabitService } from '../../core/services/habit.service';

Chart.register(MatrixController, MatrixElement);

interface HeatmapPoint {
  x: number;
  y: number;
  v: 0 | 1;
}

interface ThemePalette {
  primary: string;
  primaryShade: string;
  text: string;
  muted: string;
  grid: string;
  emptyCell: string;
  fill: string;
  tooltipBackground: string;
  tooltipBorder: string;
  surface: string;
}

@Component({
  selector: 'app-insights',
  standalone: true,
  templateUrl: './insights.component.html',
  styleUrls: ['./insights.component.scss'],
  imports: [
    IonBackButton,
    RouterLink,
    IonButtons,
    IonButton,
    IonCard,
    IonCardContent,
    IonCardHeader,
    IonCardTitle,
    IonContent,
    IonHeader,
    IonIcon,
    IonText,
    IonTitle,
    IonToolbar,
  ],
})
export class InsightsComponent implements OnInit, AfterViewInit, OnDestroy {
  private readonly destroyRef: DestroyRef = inject(DestroyRef);

  @ViewChild('heatmapCanvas')
  private heatmapCanvas?: ElementRef<HTMLCanvasElement>;

  @ViewChild('trendCanvas')
  private trendCanvas?: ElementRef<HTMLCanvasElement>;

  private chart?: Chart<'matrix', HeatmapPoint[]>;
  private trendChart?: Chart<'line', number[]>;
  private isViewReady = false;

  habits: Habit[] = [];

  constructor(
    private readonly authService: AuthService,
    private readonly habitService: HabitService,
    private readonly router: Router,
    private readonly themeService: ThemeService,
  ) {
    addIcons({ logOutOutline, moonOutline, sunnyOutline });
  }

  ngOnInit(): void {
    this.habitService
      .getTodayHabits()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((habits) => {
        this.habits = habits;

        if (this.isViewReady) {
          this.renderCharts();
        }
      });
  }

  ngAfterViewInit(): void {
    this.isViewReady = true;
    this.renderCharts();
  }

  ngOnDestroy(): void {
    this.chart?.destroy();
    this.trendChart?.destroy();
  }

  get hasHabits(): boolean {
    return this.habits.length > 0;
  }

  get weeklyCompletionRate(): number {
    if (!this.habits.length) {
      return 0;
    }

    const completed = this.habits.filter((habit) => habit.completed).length;
    return Math.round((completed / this.habits.length) * 100);
  }

  get completedHabitsCount(): number {
    return this.habits.filter((habit) => habit.completed).length;
  }

  get remainingHabitsCount(): number {
    return this.habits.filter((habit) => !habit.completed).length;
  }

  get strongestCategory(): string {
    const categories: Array<'Morning' | 'Afternoon' | 'Evening'> = ['Morning', 'Afternoon', 'Evening'];
    let bestCategory = 'Morning';
    let bestScore = -1;

    for (const category of categories) {
      const categoryHabits = this.habits.filter((habit) => habit.category === category);
      if (!categoryHabits.length) {
        continue;
      }

      const completedCount = categoryHabits.filter((habit) => habit.completed).length;
      const score = completedCount / categoryHabits.length;

      if (score > bestScore) {
        bestScore = score;
        bestCategory = category;
      }
    }

    return bestScore < 0 ? 'None yet' : bestCategory;
  }

  get strongestCategorySummary(): string {
    if (this.strongestCategory === 'None yet') {
      return 'Complete more routines to reveal a leader.';
    }

    return 'This time block is leading completion today.';
  }

  get averageQuantitativeProgress(): number {
    const quantitativeHabits = this.habits.filter((habit) => habit.type === 'Quantitative');
    if (!quantitativeHabits.length) {
      return 0;
    }

    const averageProgress =
      quantitativeHabits.reduce((total, habit) => {
        const target = habit.targetValue ?? 1;
        const current = habit.currentValue ?? 0;
        return total + current / target;
      }, 0) / quantitativeHabits.length;

    return Math.round(averageProgress * 100);
  }

  get quantitativeHabitCount(): number {
    return this.habits.filter((habit) => habit.type === 'Quantitative').length;
  }

  get quantitativeProgressSummary(): string {
    if (!this.quantitativeHabitCount) {
      return 'No measurable habits are active yet.';
    }

    return `Across ${this.quantitativeHabitCount} measurable habit${this.quantitativeHabitCount === 1 ? '' : 's'}.`;
  }

  get completionSummary(): string {
    return `${this.completedHabitsCount} of ${this.habits.length} habits completed today.`;
  }

  get pageSummary(): string {
    if (!this.hasHabits) {
      return 'Add a few habits to unlock weekly rhythm and progress trends.';
    }

    return `${this.remainingHabitsCount} habit${this.remainingHabitsCount === 1 ? '' : 's'} still need attention today.`;
  }

  get isDarkMode(): boolean {
    return this.themeService.isDarkMode();
  }

  async toggleTheme(): Promise<void> {
    await this.themeService.toggleTheme();
    this.renderCharts();
  }

  async logout(): Promise<void> {
    await this.authService.logout();
    await this.router.navigateByUrl('/login');
  }

  private renderCharts(): void {
    if (!this.hasHabits) {
      this.chart?.destroy();
      this.trendChart?.destroy();
      return;
    }

    this.renderHeatmap();
    this.renderTrend();
  }

  private renderHeatmap(): void {
    if (!this.heatmapCanvas) {
      return;
    }

    const palette = this.getThemePalette();

    this.chart?.destroy();

    this.chart = new Chart(this.heatmapCanvas.nativeElement, {
      type: 'matrix',
      data: {
        datasets: [
          {
            label: '4-week consistency',
            data: this.buildHeatmapData(),
            width: 24,
            height: 24,
            borderRadius: 8,
            borderWidth: 2,
            borderColor: palette.surface,
            backgroundColor: (context) => {
              const value = (context.raw as HeatmapPoint).v;
              return value === 1 ? palette.primaryShade : palette.emptyCell;
            },
          },
        ],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        scales: {
          x: {
            type: 'linear',
            display: false,
            min: 0.5,
            max: 4.5,
            grid: { display: false },
          },
          y: {
            type: 'linear',
            display: false,
            reverse: true,
            min: 0.5,
            max: 7.5,
            grid: { display: false },
          },
        },
        plugins: {
          legend: { display: false },
          tooltip: {
            backgroundColor: palette.tooltipBackground,
            borderColor: palette.tooltipBorder,
            borderWidth: 1,
            titleColor: palette.text,
            bodyColor: palette.text,
            callbacks: {
              title: (items) => {
                const point = items[0]?.raw as HeatmapPoint;
                return `Week ${point.x}, Day ${point.y}`;
              },
              label: (context) =>
                (context.raw as HeatmapPoint).v === 1 ? 'Completed' : 'Missed',
            },
          },
        },
      },
    });
  }

  private renderTrend(): void {
    if (!this.trendCanvas) {
      return;
    }

    const palette = this.getThemePalette();

    this.trendChart?.destroy();

    this.trendChart = new Chart(this.trendCanvas.nativeElement, {
      type: 'line',
      data: {
        labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
        datasets: [
          {
            label: 'Weekly quantitative trend',
            data: this.buildTrendData(),
            borderColor: palette.primary,
            backgroundColor: palette.fill,
            fill: true,
            tension: 0.35,
            borderWidth: 2.5,
            pointRadius: 3.5,
            pointHoverRadius: 4,
            pointBackgroundColor: palette.primary,
            pointBorderColor: palette.surface,
            pointBorderWidth: 1.5,
          },
        ],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        scales: {
          x: {
            ticks: {
              color: palette.muted,
            },
            grid: {
              display: false,
            },
            border: {
              display: false,
            },
          },
          y: {
            min: 0,
            max: 100,
            ticks: {
              color: palette.muted,
              callback: (value) => `${value}%`,
            },
            grid: {
              color: palette.grid,
            },
            border: {
              display: false,
            },
          },
        },
        plugins: {
          legend: {
            display: false,
          },
          tooltip: {
            backgroundColor: palette.tooltipBackground,
            borderColor: palette.tooltipBorder,
            borderWidth: 1,
            titleColor: palette.text,
            bodyColor: palette.text,
          },
        },
      },
    });
  }

  private getThemePalette(): ThemePalette {
    const styles = getComputedStyle(document.body);
    const read = (name: string, fallback: string): string =>
      styles.getPropertyValue(name).trim() || fallback;

    return {
      primary: read('--ion-color-primary', '#4062ff'),
      primaryShade: read('--ion-color-primary-shade', '#2d61d6'),
      text: read('--ion-text-color', '#13233d'),
      muted: read('--app-text-muted-strong', '#61728d'),
      grid: read('--app-chart-grid', 'rgba(52, 72, 113, 0.12)'),
      emptyCell: read('--app-chart-empty-cell', '#dfe8f7'),
      fill: read('--app-chart-area-fill', 'rgba(64, 98, 255, 0.18)'),
      tooltipBackground: read('--app-tooltip-background', 'rgba(15, 23, 42, 0.94)'),
      tooltipBorder: read('--app-tooltip-border', 'rgba(64, 98, 255, 0.22)'),
      surface: read('--app-surface-elevated', 'rgba(255, 255, 255, 0.96)'),
    };
  }

  private buildHeatmapData(): HeatmapPoint[] {
    const data: HeatmapPoint[] = [];
    const completionBias = this.weeklyCompletionRate / 100;

    for (let week = 1; week <= 4; week += 1) {
      for (let day = 1; day <= 7; day += 1) {
        const threshold = ((week * day) % 10) / 10;
        const completed = threshold <= completionBias;

        data.push({
          x: week,
          y: day,
          v: completed ? 1 : 0,
        });
      }
    }

    return data;
  }

  private buildTrendData(): number[] {
    const quantitativeHabits = this.habits.filter((habit) => habit.type === 'Quantitative');

    if (!quantitativeHabits.length) {
      return [0, 0, 0, 0, 0, 0, 0];
    }

    const averageProgress =
      quantitativeHabits.reduce((total, habit) => {
        const target = habit.targetValue ?? 1;
        const current = habit.currentValue ?? 0;
        return total + (current / target) * 100;
      }, 0) / quantitativeHabits.length;

    return [
      Math.max(0, averageProgress - 18),
      Math.max(0, averageProgress - 12),
      Math.max(0, averageProgress - 8),
      Math.max(0, averageProgress - 5),
      Math.max(0, averageProgress - 3),
      Math.max(0, averageProgress - 1),
      Math.min(100, averageProgress),
    ];
  }
}
