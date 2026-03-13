import { TestBed } from '@angular/core/testing';
import { HabitService } from './habit.service';

describe('HabitService', () => {
  let service: HabitService;

  beforeEach(() => {
    localStorage.removeItem('habitoo_mock_habits_v1');

    TestBed.configureTestingModule({
      providers: [HabitService],
    });

    service = TestBed.inject(HabitService);
  });

  it('should seed habits when storage is empty', () => {
    expect(service.getSnapshot().length).toBeGreaterThan(0);
  });

  it('should create and persist a mocked habit', () => {
    const createdHabit = service.createHabit({
      name: 'Write daily reflection',
      category: 'Evening',
      type: 'Binary',
    });

    const snapshot = service.getSnapshot();
    expect(snapshot.some((habit) => habit.id === createdHabit.id)).toBeTrue();

    const stored = localStorage.getItem('habitoo_mock_habits_v1') ?? '';
    expect(stored).toContain('Write daily reflection');
  });

  it('should update quantitative progress and complete a habit at target', () => {
    const habit = service.createHabit({
      name: 'Read pages',
      category: 'Afternoon',
      type: 'Quantitative',
      targetValue: 2,
    });

    service.updateHabitProgress(habit.id, 1);
    service.updateHabitProgress(habit.id, 1);

    const updatedHabit = service.getSnapshot().find((item) => item.id === habit.id);
    expect(updatedHabit?.currentValue).toBe(2);
    expect(updatedHabit?.completed).toBeTrue();
  });

  it('should remove a habit from mocked state', () => {
    const habit = service.createHabit({
      name: 'Temporary habit',
      category: 'Morning',
      type: 'Binary',
    });

    service.removeHabit(habit.id);

    const removedHabit = service.getSnapshot().find((item) => item.id === habit.id);
    expect(removedHabit).toBeUndefined();
  });
});
