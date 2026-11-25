# SecureReviewPortal

## Project Purpose
SecureReviewPortal is a capstone demo application for Section 7 of an ASP.NET Core Security course. It simulates a small internal portal where employees submit security review requests, reviewers document findings, and admins oversee remediation readiness.

## How This Project Supports Section 7 Demos
This project is structured for end-to-end secure application review activities:
- Building a review checklist
- Performing manual secure code and configuration walkthroughs
- Running AI-assisted review analysis
- Triaging findings and planning remediation
- Validating fixes and preparing release recommendations

## Prerequisites
- .NET SDK 10.0+
- EF Core CLI tools (dotnet-ef)

## How To Run The App
```powershell
dotnet restore
dotnet run
```

## How To Apply Migrations
```powershell
dotnet ef migrations add InitialCreate
dotnet ef database update
```

A startup initializer also applies migrations automatically for local demo convenience.

## Seeded Users and Roles
Development-only seeded credentials:
- Admin: admin@securereviewportal.local / DevOnly!123
- Reviewer: reviewer@securereviewportal.local / DevOnly!123
- Employee: employee@securereviewportal.local / DevOnly!123

Roles:
- Admin
- Reviewer
- Employee

## Where Uploads Are Stored
Uploaded files are saved outside wwwroot at:
- App_Data/uploads

Files are not directly served or executed.

## Intentional Review Surfaces
This app includes contained, teachable review issues for Section 7:
1. Missing reviewer policy on one reviewer-facing action:
   - ReviewFindingsController.ReviewReady lacks Reviewer/Admin role enforcement.
2. Overly verbose development-style error detail behavior:
   - Controlled by ErrorHandling:ExposeExceptionDetails in development settings.
3. Weak file upload validation:
   - FileUploadService validates extension and size only.
4. Configuration concern:
   - appsettings.Development.json includes a placeholder ExternalReviewService ApiKey.
   - This is an intentional instructional placeholder. Real secrets must not be committed to source control.
   - In production, use Secret Manager, environment variables, or a dedicated secret store such as Azure Key Vault.
   - Students will review this setting in Section 7.
5. Logging concern:
   - Submit workflow logs detailed request description and business justification.

## Secret Management Guidance
Do not place real secrets in appsettings files. For real deployments use:
- dotnet user-secrets (local development)
- Environment variables
- Production secret store (for example Azure Key Vault)

## Demo Flow For Lessons 7.2 Through 7.6
1. 7.2 Build the review checklist:
   - Use docs/security-review-checklist.md and ReviewChecklist pages.
2. 7.3 Manual secure code/config walkthrough:
   - Inspect authorization, upload, error handling, config, and logging surfaces.
3. 7.4 AI-driven security review:
   - Use docs/ai-security-review-prompt.md and compare against manual notes template.
4. 7.5 Prioritize findings and remediation planning:
   - Use docs/findings-triage-template.md and request/finding workflows.
5. 7.6 Validate fixes and release readiness:
   - Use docs/release-readiness-summary-template.md and verification checks.

## Expected Setup Flow (Course Script)
```powershell
dotnet new mvc -n SecureReviewPortal --framework net10.0
cd SecureReviewPortal
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

## Important Warning
This application is for instructional security review demonstrations and should not be deployed as-is.
