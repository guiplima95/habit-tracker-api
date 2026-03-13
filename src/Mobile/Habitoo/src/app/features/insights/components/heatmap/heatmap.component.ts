import { AfterViewInit, Component, ElementRef, OnDestroy, ViewChild } from '@angular/core';
import { Chart } from 'chart.js/auto';
import { IonCard, IonCardContent } from '@ionic/angular/standalone';
import { MatrixController, MatrixElement } from 'chartjs-chart-matrix';

Chart.register(MatrixController, MatrixElement);

interface HeatmapPoint {
  x: number;
  y: number;
  v: 0 | 1;
}

@Component({
  selector: 'app-heatmap',
  standalone: true,
  templateUrl: './heatmap.component.html',
  styleUrls: ['./heatmap.component.scss'],
  imports: [IonCard, IonCardContent],
})
export class HeatmapComponent implements AfterViewInit, OnDestroy {
  @ViewChild('heatmapCanvas', { static: true })
  private readonly heatmapCanvas!: ElementRef<HTMLCanvasElement>;

  private chart?: Chart<'matrix', HeatmapPoint[]>;

  ngAfterViewInit(): void {
    this.renderHeatmap();
  }

  ngOnDestroy(): void {
    this.chart?.destroy();
  }

  private renderHeatmap(): void {
    this.chart?.destroy();

    this.chart = new Chart(this.heatmapCanvas.nativeElement, {
      type: 'matrix',
      data: {
        datasets: [
          {
            label: 'Habit consistency',
            data: this.buildDummyData(),
            width: 16,
            height: 16,
            borderRadius: 3,
            backgroundColor: (context) => {
              const value = (context.raw as HeatmapPoint).v;
              return value === 1 ? '#2dd36f' : '#e7ebf3';
            },
          },
        ],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        animation: {
          duration: 500,
        },
        scales: {
          x: {
            type: 'linear',
            display: false,
            min: 0.5,
            max: 12.5,
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
            callbacks: {
              title: () => 'Dummy Habit Data',
              label: (context) =>
                (context.raw as HeatmapPoint).v === 1 ? 'Completed' : 'Missed',
            },
          },
        },
      },
    });
  }

  private buildDummyData(): HeatmapPoint[] {
    const data: HeatmapPoint[] = [];

    for (let week = 1; week <= 12; week += 1) {
      for (let day = 1; day <= 7; day += 1) {
        const completed = (week + day) % 3 === 0 || week % 4 === 0;

        data.push({
          x: week,
          y: day,
          v: completed ? 1 : 0,
        });
      }
    }

    return data;
  }
}
