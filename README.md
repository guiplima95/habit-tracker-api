<h1 align="center">
  <img alt="Habitoo Logo" src="https://via.placeholder.com/150x150.png?text=Habitoo+Logo" width="120px" />
  <br/>
  Habitoo - Next-Generation Habit Tracker
</h1>

<p align="center">
  <b>Habitoo</b> is a full-stack, mobile-first habit tracking platform engineered with behavioral psychology principles to help users build consistency and achieve long-term goals.
</p>

<p align="center">
  <img alt="GitHub language count" src="https://img.shields.io/github/languages/count/guiplima95/habit-tracker-api">
  <img alt="Repository size" src="https://img.shields.io/github/repo-size/guiplima95/habit-tracker-api">
  <img alt="License" src="https://img.shields.io/github/license/guiplima95/habit-tracker-api">
  <img alt=".NET Version" src="https://img.shields.io/badge/.NET-8.0+-512BD4?style=flat&logo=dotnet&logoColor=white">
  <img alt="Angular Version" src="https://img.shields.io/badge/Angular-19+-DD0031?style=flat&logo=angular&logoColor=white">
  <img alt="Ionic Version" src="https://img.shields.io/badge/Ionic-8+-3880FF?style=flat&logo=ionic&logoColor=white">
</p>

<p align="center">
  <a href="#-about-the-project">About</a>&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
  <a href="#-features">Features</a>&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
  <a href="#-technologies">Technologies</a>&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
  <a href="#-architecture--monorepo-structure">Architecture</a>&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
  <a href="#-how-to-run">How to Run</a>
</p>

<br>

## 📖 About the Project

**Habitoo** is a comprehensive platform designed for individuals seeking a clean, data-driven approach to tracking their daily routines. Built around the concept of the "Habit Loop" and the "Two-Minute Rule", the app minimizes friction and maximizes consistency.

This repository is a **Monorepo** containing both the robust **.NET Web API backend** and the cross-platform **Ionic/Angular frontend**. By centralizing the codebase, we ensure seamless data synchronization, shared architectural context, and unified deployment pipelines.

---

## ✨ Features

### Frontend (Mobile & Web)
- **Smart Dashboard:** Habits are contextually grouped by time of day (Morning, Afternoon, Evening) using an intuitive UI.
- **Contribution Heatmap:** A GitHub-style progress matrix powered by `Chart.js`, visualizing consistency over months.
- **Offline-First Synchronization:** Logs are queued locally via Capacitor Storage and synced when internet connectivity is restored.
- **Dual Authentication:** Social login via Google (Capacitor Google Auth) and standard email registration using JWT.
- **Quantitative & Binary Tracking:** Track "Yes/No" habits alongside numerical goals (e.g., "Read 20 pages") with visual progress bars.

### Backend (API & Services)
- **Centralized Streak Engine:** Streak logic, grace periods, and "streak freezes" are calculated securely on the server to prevent timezone manipulation.
- **Multi-Channel Notifications:** Background workers schedule and dispatch contextual reminders via WhatsApp (Twilio) and Email (SendGrid).
- **Scalable Data Modeling:** Relational database handling users, habit configurations, daily logs, and dynamic achievements.
- **Secure Identity Management:** Role-based access control and secure JWT generation and validation.

---

## 🛠️ Technologies

### 🖥️ Frontend Stack
- **[Angular 19+](https://angular.dev/)** - Core framework using Standalone Components.
- **[Ionic 8](https://ionicframework.com/)** - Native UI components for cross-platform deployment (iOS, Android, Web).
- **[Capacitor](https://capacitorjs.com/)** - Runtime for accessing native device APIs.
- **[Chart.js](https://www.chartjs.org/)** - Lightweight library for rendering analytical charts and heatmaps.

### ⚙️ Backend Stack
- **[.NET 8+ / C#](https://dotnet.microsoft.com/)** - High-performance Web API framework.
- **[Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)** - ORM for robust database management (PostgreSQL/SQLite).
- **[Twilio API](https://www.twilio.com/)** - Integration for automated WhatsApp reminders.
- **[SendGrid](https://sendgrid.com/)** - Transactional email delivery for reports and alerts.
- **[Swagger / OpenAPI](https://swagger.io/)** - API documentation and testing interface.

---

## 📂 Architecture & Monorepo Structure

The project is divided into two main environments residing in the same repository:

```text
habitoo/
 ├── backend/               # .NET Core Web API
 │   ├── Controllers/       # API Endpoints (Auth, Habits, Logs)
 │   ├── Models/            # Entity models and DTOs
 │   ├── Data/              # EF Core DbContext and Migrations
 │   └── Services/          # Business logic (StreakEngine, NotificationService)
 │
 ├── frontend/              # Ionic + Angular Application
 │   ├── src/app/core/      # Singleton services (AuthService, SyncQueue) and Interceptors
 │   ├── src/app/shared/    # Reusable UI components (HabitCard, HeatmapChart)
 │   └── src/app/features/  # Main application views (Dashboard, Insights, Settings)
 │
 └── README.md

--------------------------------------------------------------------------------
🚀 How to Run
Prerequisites
Node.js (LTS) & Ionic CLI (npm i -g @ionic/cli)
.NET SDK (v8.0 or higher)
A Database (SQLite is configured by default for development)
1. Running the Backend API
# Navigate to the backend directory
$ cd backend

# Restore .NET dependencies
$ dotnet restore

# Apply database migrations
$ dotnet ef database update

# Run the API
$ dotnet run
The Swagger documentation will be available at http://localhost:5000/swagger.
2. Running the Frontend App
Open a new terminal window:
# Navigate to the frontend directory
$ cd frontend

# Install npm dependencies
$ npm install

# Run the application in development mode
$ ionic serve
The app will automatically open in your browser at http://localhost:8100.