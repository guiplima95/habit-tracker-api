import { CommonModule } from '@angular/common';
import { Component, DestroyRef, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import {
  IonAccordion,
  IonAccordionGroup,
  IonButton,
  IonButtons,
  IonCard,
  IonCardContent,
  IonCardHeader,
  IonCardTitle,
  IonCol,
  IonContent,
  IonFab,
  IonFabButton,
  IonGrid,
  IonHeader,
  IonIcon,
  IonInput,
  IonItem,
  IonLabel,
  IonList,
  IonModal,
  IonRow,
  IonSegment,
  IonSegmentButton,
  IonSelect,
  IonSelectOption,
  IonText,
  IonTitle,
  IonToolbar,
  ToastController,
} from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import { add, analyticsOutline, logOutOutline, moonOutline, sunnyOutline } from 'ionicons/icons';
import { AuthService } from '../../core/services/auth.service';
import { ThemeService } from '../../core/services/theme.service';
import { HabitItemComponent } from '../../shared/components/habit-item/habit-item.component';
import { Habit, HabitCategory, HabitType } from '../../shared/models/habit.model';
import { CreateHabitInput, HabitService } from '../../core/services/habit.service';

type CreateHabitForm = FormGroup<{
  name: FormControl<string>;
  category: FormControl<HabitCategory>;
  type: FormControl<HabitType>;
  targetValue: FormControl<number>;
  customSchedule: FormControl<string>;
}>;

@Component({
  selector: 'app-home',
  standalone: true,
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  imports: [
    CommonModule,
    RouterLink,
    ReactiveFormsModule,
    HabitItemComponent,
    IonAccordion,
    IonAccordionGroup,
    IonButton,
    IonButtons,
    IonCard,
    IonCardContent,
    IonCardHeader,
    IonCardTitle,
    IonCol,
    IonContent,
    IonFab,
    IonFabButton,
    IonGrid,
    IonHeader,
    IonIcon,
    IonInput,
    IonItem,
    IonLabel,
    IonList,
    IonModal,
    IonRow,
    IonSegment,
    IonSegmentButton,
    IonSelect,
    IonSelectOption,
    IonText,
    IonTitle,
    IonToolbar,
  ],
})
export class HomeComponent implements OnInit {
  private readonly destroyRef: DestroyRef = inject(DestroyRef);

  readonly categories: HabitCategory[] = ['Morning', 'Afternoon', 'Evening'];
  readonly habitTypes: HabitType[] = ['Binary', 'Quantitative'];

  selectedCategory: HabitCategory = 'Morning';
  habits: Habit[] = [];
  isCreateModalOpen = false;

  readonly createHabitForm: CreateHabitForm = new FormGroup({
    name: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(80)],
    }),
    category: new FormControl<HabitCategory>('Morning', {
      nonNullable: true,
      validators: [Validators.required],
    }),
    type: new FormControl<HabitType>('Binary', {
      nonNullable: true,
      validators: [Validators.required],
    }),
    targetValue: new FormControl(10, {
      nonNullable: true,
      validators: [Validators.min(1)],
    }),
    customSchedule: new FormControl('', {
      nonNullable: true,
    }),
  });

  constructor(
    private readonly authService: AuthService,
    private readonly habitService: HabitService,
    private readonly router: Router,
    private readonly themeService: ThemeService,
    private readonly toastController: ToastController,
  ) {
    addIcons({ add, analyticsOutline, logOutOutline, moonOutline, sunnyOutline });
  }

  ngOnInit(): void {
    this.habitService
      .getTodayHabits()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((todayHabits) => {
        this.habits = todayHabits;
      });
  }

  get filteredHabits(): Habit[] {
    return this.habits.filter((habit) => habit.category === this.selectedCategory);
  }

  get completionRatio(): number {
    if (!this.habits.length) {
      return 0;
    }

    const completedCount = this.habits.filter((habit) => habit.completed).length;
    return completedCount / this.habits.length;
  }

  get remainingHabitsCount(): number {
    return this.habits.filter((habit) => !habit.completed).length;
  }

  get completedHabitsCount(): number {
    return this.habits.filter((habit) => habit.completed).length;
  }

  get currentStreak(): number {
    const completed = this.habits.filter((habit) => habit.completed).length;
    return completed > 0 ? completed + 2 : 0;
  }

  get greeting(): string {
    const hour = new Date().getHours();

    if (hour < 12) {
      return 'Good Morning';
    }

    if (hour < 18) {
      return 'Good Afternoon';
    }

    return 'Good Evening';
  }

  get focusSummary(): string {
    const count = this.filteredHabits.length;
    const label = count === 1 ? '1 habit scheduled' : `${count} habits scheduled`;
    return `${label} for ${this.selectedCategory.toLowerCase()}.`;
  }

  get isDarkMode(): boolean {
    return this.themeService.isDarkMode();
  }

  onSegmentChange(event: CustomEvent<{ value?: string | number }>): void {
    const nextValue = event.detail.value;

    if (nextValue === 'Morning' || nextValue === 'Afternoon' || nextValue === 'Evening') {
      this.selectedCategory = nextValue;
    }
  }

  async markHabitCompleted(habitId: number): Promise<void> {
    this.habitService.completeHabit(habitId);
    await this.presentToast('Habit completed. Great momentum!', 'success');
  }

  async adjustQuantitative(event: { habitId: number; delta: number }): Promise<void> {
    this.habitService.updateHabitProgress(event.habitId, event.delta);

    const changedHabit = this.habitService.getSnapshot().find((habit) => habit.id === event.habitId);
    if (changedHabit?.completed) {
      await this.presentToast('Target reached. Nice work!', 'success');
    }
  }

  async removeHabit(habitId: number): Promise<void> {
    this.habitService.removeHabit(habitId);
    await this.presentToast('Habit removed from today.', 'medium');
  }

  openCreateHabitModal(): void {
    this.isCreateModalOpen = true;
    this.createHabitForm.patchValue({
      category: this.selectedCategory,
    });
  }

  closeCreateHabitModal(): void {
    this.isCreateModalOpen = false;
    this.createHabitForm.reset({
      name: '',
      category: this.selectedCategory,
      type: 'Binary',
      targetValue: 10,
      customSchedule: '',
    });
  }

  async createHabit(): Promise<void> {
    if (this.createHabitForm.invalid) {
      this.createHabitForm.markAllAsTouched();
      return;
    }

    const habitInput: CreateHabitInput = {
      name: this.createHabitForm.controls.name.value,
      category: this.createHabitForm.controls.category.value,
      type: this.createHabitForm.controls.type.value,
      targetValue: this.createHabitForm.controls.targetValue.value,
      customSchedule: this.createHabitForm.controls.customSchedule.value,
    };

    const createdHabit = this.habitService.createHabit(habitInput);
    this.selectedCategory = createdHabit.category;
    this.closeCreateHabitModal();
    await this.presentToast('Habit created and ready for today.', 'primary');
  }

  async toggleTheme(): Promise<void> {
    await this.themeService.toggleTheme();
  }

  async logout(): Promise<void> {
    this.closeCreateHabitModal();
    await this.authService.logout();
    await this.router.navigateByUrl('/login');
  }

  private async presentToast(
    message: string,
    color: 'success' | 'medium' | 'primary',
  ): Promise<void> {
    const toast = await this.toastController.create({
      message,
      duration: 1200,
      position: 'bottom',
      color,
    });

    await toast.present();
  }
}
