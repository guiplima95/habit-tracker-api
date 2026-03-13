export type HabitCategory = 'Morning' | 'Afternoon' | 'Evening';
export type HabitType = 'Binary' | 'Quantitative';

export interface Habit {
  id: number;
  name: string;
  category: HabitCategory;
  type: HabitType;
  completed: boolean;
  currentValue?: number;
  targetValue?: number;
  customSchedule?: string;
}
