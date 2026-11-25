# Manual Review Notes Template

| Area reviewed | File or workflow | Observation | Evidence | Possible risk | Follow-up needed |
|---|---|---|---|---|---|
| Error handling and logging | Program.cs | Production error behavior needs release-readiness verification. | Error-handling configuration is environment-dependent | Incorrect production behavior could expose implementation details or create poor failure handling. | Validate production behavior |
| Authentication and authorization | SecurityReviewRequestsController | Employee-owned request workflows appear to include ownership checks. | Actions compare the current user or role against the request owner before returning sensitive data. | Ownership checks reduce the risk of unauthorized access to another user’s request. | Validate with user-role testing later. |
| Authentication and authorization | ReviewFindingsController.Create | Reviewer-facing action appears to require authentication but not Reviewer/Admin enforcement. | The action is protected by general authentication but does not show role or policy restriction for Reviewer/Admin. | A normal authenticated user may be able to access functionality intended for reviewers. | Confirm behavior in browser, then prioritize for remediation. |
| Request and input handling | FileUploadService | Upload validation exists but appears limited to size, extension, and possibly browser-provided content type. | FileUploadService checks allowed extensions and maximum length, but does not deeply validate file content. | Extension-only or content-type-based validation can be bypassed or may misrepresent the actual file type. | Strengthen validation during remediation and validate download authorization. |
| Request and input handling | FileUploadService | Uploaded files appear to be stored outside wwwroot. | Upload path uses an application-controlled storage location rather than the public web root. | This reduces direct public exposure of uploaded files. | Confirm download access is authorized. |
| Authentication and authorization | ReviewAttachmentsController.Download | Attachment downloads should be validated against request ownership and reviewer/admin access. | Download workflow references request ownership or role checks before returning the file. | Missing checks could expose uploaded supporting documents. | Validate using Employee, Reviewer, and Admin users. |
| Error handling and logging | Review request submission workflow | Logging may capture more business context than necessary. | Log statement includes request title, business justification, filename, or similar workflow detail. | Logs may expose sensitive business context beyond the application database. | Reduce log detail during remediation while preserving useful audit information. |
| Configuration and environment safety | appsettings.Development.json | Development placeholder API key setting should not become a production secret-management pattern. | ExternalReviewService includes a fake ApiKey value in development configuration. | Real API keys committed to configuration files can lead to credential exposure. | Confirm real secrets use Secret Manager, environment variables, or production secret storage. |

| Surface | Expected Access | Notes |
|---|---|---|
| Public pages | Anonymous | Home, login, registration, general pages |
| Review requests | Employee / Reviewer / Admin | Employees should only access their own requests |
| Review findings | Reviewer / Admin | Findings should not be created by normal employees |
| Attachments | Owner / Reviewer / Admin | Upload and download need careful authorization |
| Checklist management | Admin | Administrative review setup |
| Admin area | Admin | Highest privilege surface |

### Likely Findings

1. Reviewer-facing finding action may need stronger Reviewer/Admin role enforcement.
2. File upload validation exists but should be strengthened beyond extension and content-type checks.
3. Logging may capture excessive business context in a security-sensitive workflow.
4. Development placeholder API key should trigger secret-management review.
5. Production error-handling behavior should be validated before release.

### Positive Observations

1. Authentication and authorization middleware ordering appears correct.
2. Identity and role-based structure are present.
3. Employee-owned request workflows appear to include ownership checks.
4. Uploaded files appear to be stored outside wwwroot.
5. Administrative surfaces appear to use Admin role restrictions.

### Follow-Up Needed

1. Validate reviewer-only access in the browser.
2. Validate attachment download authorization.
3. Reduce excessive logging detail during remediation.
4. Confirm production-safe configuration and secret handling.
5. Validate production error behavior before release.