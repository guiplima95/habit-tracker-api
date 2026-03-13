import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import {
  IonButton,
  IonBadge,
  IonIcon,
  IonItem,
  IonItemOption,
  IonItemOptions,
  IonItemSliding,
  IonLabel,
  IonProgressBar,
} from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import { add, checkmark, remove, trashOutline } from 'ionicons/icons';
import { Habit } from '../../models/habit.model';

@Component({
  selector: 'app-habit-item',
  standalone: true,
  templateUrl: './habit-item.component.html',
  styleUrls: ['./habit-item.component.scss'],
  imports: [
    CommonModule,
    IonButton,
    IonBadge,
    IonIcon,
    IonItem,
    IonItemOption,
    IonItemOptions,
    IonItemSliding,
    IonLabel,
    IonProgressBar,
  ],
})
export class HabitItemComponent {
  @Input({ required: true }) habit!: Habit;
  @Output() readonly habitCompleted = new EventEmitter<number>();
  @Output() readonly quantitativeAdjusted = new EventEmitter<{ habitId: number; delta: number }>();
  @Output() readonly habitRemoved = new EventEmitter<number>();

  constructor() {
    addIcons({ add, checkmark, remove, trashOutline });
  }

  completeHabit(slidingItem: IonItemSliding): void {
    if (!this.habit.completed) {
      this.habitCompleted.emit(this.habit.id);
    }

    void slidingItem.close();
  }

  adjustQuantitative(delta: number): void {
    if (this.habit.type !== 'Quantitative') {
      return;
    }

    this.quantitativeAdjusted.emit({
      habitId: this.habit.id,
      delta,
    });
  }

  removeHabit(): void {
    this.habitRemoved.emit(this.habit.id);
  }

  get statusLabel(): string {
    if (this.habit.completed) {
      return 'Completed';
    }

    if (this.habit.type === 'Quantitative' && (this.habit.currentValue ?? 0) > 0) {
      return 'In progress';
    }

    return 'Pending';
  }

  get statusColor(): 'success' | 'warning' | 'medium' {
    if (this.habit.completed) {
      return 'success';
    }

    if (this.habit.type === 'Quantitative' && (this.habit.currentValue ?? 0) > 0) {
      return 'warning';
    }

    return 'medium';
  }

  get progressValue(): number {
    if (this.habit.type !== 'Quantitative') {
      return this.habit.completed ? 1 : 0;
    }

    const target = this.habit.targetValue ?? 1;
    const current = this.habit.currentValue ?? 0;
    return Math.max(0, Math.min(1, current / target));
  }
}
