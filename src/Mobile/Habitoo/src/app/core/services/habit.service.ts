import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Habit, HabitCategory, HabitType } from '../../shared/models/habit.model';

export interface CreateHabitInput {
  name: string;
  category: HabitCategory;
  type: HabitType;
  targetValue?: number;
  customSchedule?: string;
}

@Injectable({
  providedIn: 'root',
})
export class HabitService {
  private static readonly StorageKey = 'habitoo_mock_habits_v1';

  private readonly habitsSubject = new BehaviorSubject<Habit[]>(this.loadHabits());
  private nextHabitId = this.habitsSubject.value.reduce((max, habit) => Math.max(max, habit.id), 0) + 1;

  getTodayHabits(): Observable<Habit[]> {
    return this.habitsSubject.asObservable();
  }

  createHabit(input: CreateHabitInput): Habit {
    const nextHabit: Habit = {
      id: this.nextHabitId,
      name: input.name.trim(),
      category: input.category,
      type: input.type,
      completed: false,
      targetValue: input.type === 'Quantitative' ? Math.max(1, input.targetValue ?? 1) : undefined,
      currentValue: input.type === 'Quantitative' ? 0 : undefined,
      customSchedule: input.customSchedule?.trim() || undefined,
    };

    this.nextHabitId += 1;
    this.updateHabits([nextHabit, ...this.habitsSubject.value]);
    return nextHabit;
  }

  completeHabit(habitId: number): void {
    const habits = this.habitsSubject.value.map((habit) => {
      if (habit.id !== habitId) {
        return habit;
      }

      return {
        ...habit,
        completed: true,
        currentValue: habit.type === 'Quantitative' ? habit.targetValue : habit.currentValue,
      };
    });

    this.updateHabits(habits);
  }

  updateHabitProgress(habitId: number, delta: number): void {
    const habits = this.habitsSubject.value.map((habit) => {
      if (habit.id !== habitId || habit.type !== 'Quantitative') {
        return habit;
      }

      const target = habit.targetValue ?? 1;
      const current = habit.currentValue ?? 0;
      const nextValue = Math.max(0, Math.min(target, current + delta));

      return {
        ...habit,
        currentValue: nextValue,
        completed: nextValue >= target,
      };
    });

    this.updateHabits(habits);
  }

  removeHabit(habitId: number): void {
    this.updateHabits(this.habitsSubject.value.filter((habit) => habit.id !== habitId));
  }

  getSnapshot(): Habit[] {
    return [...this.habitsSubject.value];
  }

  private updateHabits(habits: Habit[]): void {
    this.habitsSubject.next(habits);
    this.persistHabits(habits);
  }

  private loadHabits(): Habit[] {
    try {
      const rawValue = localStorage.getItem(HabitService.StorageKey);
      if (rawValue) {
        const parsedValue = JSON.parse(rawValue) as Habit[];
        if (Array.isArray(parsedValue) && parsedValue.length) {
          return parsedValue;
        }
      }
    } catch {
      // Fall back to seeded habits when local storage is unavailable.
    }

    return this.seedHabits();
  }

  private persistHabits(habits: Habit[]): void {
    try {
      localStorage.setItem(HabitService.StorageKey, JSON.stringify(habits));
    } catch {
      // Ignore persistence failures in mock mode.
    }
  }

  private seedHabits(): Habit[] {
    return [
      {
        id: 1,
        name: 'Drink 2L Water',
        category: 'Morning',
        type: 'Quantitative',
        completed: false,
        currentValue: 1,
        targetValue: 2,
      },
      {
        id: 2,
        name: 'Read 10 pages',
        category: 'Afternoon',
        type: 'Quantitative',
        completed: false,
        currentValue: 4,
        targetValue: 10,
      },
      {
        id: 3,
        name: 'Stretch for 15 minutes',
        category: 'Evening',
        type: 'Binary',
        completed: true,
      },
      {
        id: 4,
        name: 'Plan tomorrow priorities',
        category: 'Evening',
        type: 'Binary',
        completed: false,
      },
    ];
  }
}
