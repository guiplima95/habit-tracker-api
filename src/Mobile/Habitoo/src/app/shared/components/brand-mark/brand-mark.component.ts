import { Component } from '@angular/core';

@Component({
  selector: 'app-brand-mark',
  standalone: true,
  template: `
    <div class="mark-shell" aria-hidden="true">
      <div class="mark-aura"></div>

      <svg viewBox="0 0 160 160" class="mark-icon" focusable="false">
        <path class="mark-orbit mark-orbit-primary" d="M40 62c7-17 23-28 41-28 24 0 44 20 44 44 0 7-2 15-5 21" />
        <path class="mark-orbit mark-orbit-secondary" d="M121 99c-8 17-24 28-42 28-24 0-44-20-44-44 0-7 2-14 5-20" />
        <rect class="mark-pill" x="48" y="48" width="19" height="62" rx="9.5" />
        <rect class="mark-pill" x="95" y="48" width="19" height="62" rx="9.5" />
        <rect class="mark-bridge" x="64" y="68" width="34" height="22" rx="11" />
        <circle class="mark-badge" cx="117" cy="110" r="18" />
        <path class="mark-check" d="M108 110l7 7 14-17" />
      </svg>
    </div>
  `,
  styles: [
    `
      :host {
        display: block;
      }

      .mark-shell {
        width: 100%;
        height: 100%;
        display: grid;
        place-items: center;
        position: relative;
        border-radius: 32%;
        overflow: hidden;
        background:
          radial-gradient(circle at 24% 20%, rgba(255, 255, 255, 0.3), transparent 26%),
          radial-gradient(circle at 78% 82%, rgba(161, 247, 233, 0.26), transparent 28%),
          linear-gradient(150deg, #2756e6 0%, #3f76ff 46%, #5ed3e4 100%);
        box-shadow:
          inset 0 1px 0 rgba(255, 255, 255, 0.34),
          0 22px 34px rgba(23, 51, 130, 0.3);
      }

      .mark-aura {
        position: absolute;
        inset: 18%;
        border-radius: 32%;
        background: rgba(255, 255, 255, 0.14);
        filter: blur(18px);
      }

      .mark-shell::after {
        content: '';
        position: absolute;
        inset: 9%;
        border-radius: 28%;
        border: 1px solid rgba(255, 255, 255, 0.18);
      }

      .mark-icon {
        width: 82%;
        height: 82%;
        display: block;
        position: relative;
        z-index: 1;
      }

      .mark-orbit {
        fill: none;
        stroke-linecap: round;
        stroke-width: 9;
      }

      .mark-orbit-primary {
        stroke: rgba(255, 255, 255, 0.9);
      }

      .mark-orbit-secondary {
        stroke: rgba(176, 245, 233, 0.82);
      }

      .mark-pill,
      .mark-bridge {
        fill: rgba(255, 255, 255, 0.98);
      }

      .mark-badge {
        fill: #0e1e46;
        stroke: rgba(255, 255, 255, 0.16);
        stroke-width: 2;
      }

      .mark-check {
        fill: none;
        stroke: #99f7df;
        stroke-width: 6.5;
        stroke-linecap: round;
        stroke-linejoin: round;
      }

      :host-context(body.theme-dark) .mark-shell {
        box-shadow:
          inset 0 1px 0 rgba(255, 255, 255, 0.2),
          0 22px 38px rgba(0, 0, 0, 0.42);
      }
    `,
  ],
})
export class BrandMarkComponent {}
