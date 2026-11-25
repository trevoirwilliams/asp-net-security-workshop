# AspNetCoreSecuritySection6

A baseline ASP.NET Core MVC task management application built for a security training course on **Threat Modeling and Secure Design**.

> **Purpose:** This is an intentionally baseline application — it demonstrates realistic application surfaces without advanced security hardening. It is used as the starting point for threat modeling exercises, STRIDE analysis, and secure design rework in later course sections.

---

## Features

| Feature | Details |
|---|---|
| Authentication | ASP.NET Core Identity (email/password, no email confirmation) |
| Roles | `Admin`, `User` |
| Task management | Create, list, view details, edit, mark complete |
| Task comments | Authenticated users can comment on any accessible task |
| Task attachments | File upload stored under `wwwroot/uploads/`; metadata in DB |
| Admin panel | Admin can view all users and all tasks |
| CSV export | Authenticated endpoint exports task data as `.csv` |
| JSON API | `GET /api/tasks` returns tasks as JSON (authenticated) |
| Webhook integration | On task completion, an outbound HTTP POST is sent to a configurable URL |

---

## Seeded Users

| Role | Email | Password |
|---|---|---|
| Admin | `admin@AspNetCoreSecuritySection6.local` | `Admin@123!` |
| User | `alice@AspNetCoreSecuritySection6.local` | `Alice@123!` |

---

## How to Run

**Prerequisites:** .NET 10 SDK

```bash
cd AspNetCoreSecuritySection6
dotnet run
```

The database is automatically created and seeded on first launch via EF Core migrations.

Browse to `https://localhost:<port>` (check `Properties/launchSettings.json` for the exact port).

---

## Configuration

`appsettings.json` contains:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "DataSource=app.db;Cache=Shared"
  },
  "WebhookSettings": {
    "CompletionUrl": ""
  }
}
```

Set `CompletionUrl` to any URL to receive a JSON POST when a task is marked complete.

---

## Application Surfaces for Threat Modeling

The following areas are the primary surfaces to analyze:

| Surface | Location | Notes |
|---|---|---|
| **Authentication** | `Areas/Identity/Pages/Account/` | Register, Login, Logout via ASP.NET Core Identity |
| **Authorization** | All controllers | Role-checked with `[Authorize]` and `[Authorize(Roles="Admin")]` |
| **Task CRUD** | `Controllers/TasksController.cs` | Owner-scoped; admin can access all |
| **File upload** | `TasksController.UploadAttachment()` | Files saved to `wwwroot/uploads/` using a GUID filename; original filename stored in DB |
| **File download** | `/uploads/<storedFileName>` | Served as static files — no auth check at the file level |
| **CSV export** | `Controllers/ExportController.cs` | `GET /Export/Tasks` — returns CSV of tasks |
| **JSON API** | `Controllers/Api/TasksController.cs` | `GET /api/tasks` — returns JSON |
| **Webhook** | `Services/WebhookService.cs` | Outbound HTTP POST on task completion; URL from config |
| **Admin panel** | `Controllers/AdminController.cs` | Lists all users and all tasks |
| **Seeded secrets** | `Services/SeedDataService.cs` | Hardcoded default credentials for demo |

---

## Project Structure

```
AspNetCoreSecuritySection6/
├── Controllers/
│   ├── HomeController.cs
│   ├── DashboardController.cs
│   ├── TasksController.cs           ← task CRUD, comments, uploads
│   ├── AdminController.cs           ← admin panel
│   ├── ExportController.cs          ← CSV export
│   └── Api/
│       └── TasksController.cs       ← JSON API
├── Data/
│   └── ApplicationDbContext.cs
├── Models/
│   ├── ApplicationUser.cs
│   ├── TaskItem.cs
│   ├── TaskComment.cs
│   └── TaskAttachment.cs
├── Services/
│   ├── SeedDataService.cs
│   └── WebhookService.cs            ← external integration
├── Views/
│   ├── Home/
│   ├── Dashboard/
│   ├── Tasks/
│   ├── Admin/
│   └── Shared/
└── wwwroot/
    └── uploads/                     ← file attachments stored here
```
