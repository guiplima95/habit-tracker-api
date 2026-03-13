import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import {
  IonAccordion,
  IonAccordionGroup,
  IonButton,
  IonButtons,
  IonCard,
  IonCardContent,
  IonCardHeader,
  IonCardTitle,
  IonContent,
  IonFab,
  IonFabButton,
  IonHeader,
  IonIcon,
  IonInput,
  IonItem,
  IonLabel,
  IonList,
  IonModal,
  IonProgressBar,
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
import { add } from 'ionicons/icons';
import { HabitItemComponent } from '../../shared/components/habit-item/habit-item.component';
import { Habit, HabitCategory, HabitType } from '../../shared/models/habit.model';

type AddHabitForm = FormGroup<{
  name: FormControl<string>;
  category: FormControl<HabitCategory>;
  type: FormControl<HabitType>;
  targetValue: FormControl<number>;
  customSchedule: FormControl<string>;
}>;

@Component({
  selector: 'app-dashboard',
  standalone: true,
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
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
    IonContent,
    IonFab,
    IonFabButton,
    IonHeader,
    IonIcon,
    IonInput,
    IonItem,
    IonLabel,
    IonList,
    IonModal,
    IonProgressBar,
    IonSegment,
    IonSegmentButton,
    IonSelect,
    IonSelectOption,
    IonText,
    IonTitle,
    IonToolbar,
  ],
})
export class DashboardComponent {
  readonly categories: HabitCategory[] = ['Morning', 'Afternoon', 'Evening'];
  readonly habitTypes: HabitType[] = ['Binary', 'Quantitative'];

  selectedCategory: HabitCategory = 'Morning';
  isAddHabitModalOpen = false;

  readonly addHabitForm: AddHabitForm = new FormGroup({
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
    targetValue: new FormControl(20, {
      nonNullable: true,
      validators: [Validators.min(1)],
    }),
    customSchedule: new FormControl('', {
      nonNullable: true,
    }),
  });

  private readonly recentCompletionHistory: boolean[] = [
    true,
    true,
    false,
    true,
    true,
    true,
    false,
    true,
    true,
  ];

  habits: Habit[] = [
    {
      id: 1,
      name: 'Journal for 5 minutes',
      category: 'Morning',
      type: 'Binary',
      completed: false,
    },
    {
      id: 2,
      name: 'Drink 2L of water',
      category: 'Afternoon',
      type: 'Quantitative',
      completed: false,
      currentValue: 1,
      targetValue: 2,
    },
    {
      id: 3,
      name: 'Read 10 pages',
      category: 'Evening',
      type: 'Binary',
      completed: false,
    },
  ];

  private nextHabitId = this.habits.length + 1;

  constructor(private readonly toastController: ToastController) {
    addIcons({ add });
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

  get completionPercentageLabel(): string {
    return `${Math.round(this.completionRatio * 100)}% complete today`;
  }

  get currentStreakDays(): number {
    let streak = 0;

    for (const dayCompleted of this.recentCompletionHistory) {
      if (!dayCompleted) {
        break;
      }

      streak += 1;
    }

    return streak;
  }

  onSegmentChange(event: CustomEvent<{ value?: string | number }>): void {
    const selectedValue = event.detail.value;

    if (
      selectedValue === 'Morning' ||
      selectedValue === 'Afternoon' ||
      selectedValue === 'Evening'
    ) {
      this.selectedCategory = selectedValue;
    }
  }

  async markHabitCompleted(habitId: number): Promise<void> {
    let shouldToast = false;

    this.habits = this.habits.map((habit) => {
      if (habit.id !== habitId) {
        return habit;
      }

      if (!habit.completed) {
        shouldToast = true;
      }

      return {
        ...habit,
        completed: true,
        currentValue: habit.type === 'Quantitative' ? habit.targetValue : habit.currentValue,
      };
    });

    if (shouldToast) {
      await this.presentCompletionToast();
    }
  }

  adjustQuantitativeValue(event: { habitId: number; delta: number }): void {
    this.habits = this.habits.map((habit) => {
      if (habit.id !== event.habitId || habit.type !== 'Quantitative') {
        return habit;
      }

      const target = habit.targetValue ?? 1;
      const current = habit.currentValue ?? 0;
      const nextValue = Math.max(0, Math.min(target, current + event.delta));

      return {
        ...habit,
        currentValue: nextValue,
        completed: nextValue >= target,
      };
    });
  }

  openAddHabitModal(): void {
    this.isAddHabitModalOpen = true;
    this.addHabitForm.patchValue({
      category: this.selectedCategory,
    });
  }

  closeAddHabitModal(): void {
    this.isAddHabitModalOpen = false;
    this.addHabitForm.reset({
      name: '',
      category: this.selectedCategory,
      type: 'Binary',
      targetValue: 20,
      customSchedule: '',
    });
  }

  createHabit(): void {
    if (this.addHabitForm.invalid) {
      this.addHabitForm.markAllAsTouched();
      return;
    }

    const newHabitName = this.addHabitForm.controls.name.value.trim();
    if (!newHabitName) {
      this.addHabitForm.controls.name.setErrors({ required: true });
      return;
    }

    const category = this.addHabitForm.controls.category.value;
    const type = this.addHabitForm.controls.type.value;
    const targetValue = this.addHabitForm.controls.targetValue.value;
    const customSchedule = this.addHabitForm.controls.customSchedule.value.trim();

    const newHabit: Habit = {
      id: this.nextHabitId,
      name: newHabitName,
      category,
      type,
      completed: false,
      customSchedule: customSchedule || undefined,
    };

    if (type === 'Quantitative') {
      newHabit.currentValue = 0;
      newHabit.targetValue = Math.max(1, targetValue);
    }

    this.nextHabitId += 1;
    this.habits = [newHabit, ...this.habits];
    this.selectedCategory = category;
    this.closeAddHabitModal();
  }

  private async presentCompletionToast(): Promise<void> {
    const toast = await this.toastController.create({
      message: 'Great job! Keep the streak alive.',
      duration: 1300,
      position: 'bottom',
      color: 'success',
    });

    await toast.present();
  }
}
