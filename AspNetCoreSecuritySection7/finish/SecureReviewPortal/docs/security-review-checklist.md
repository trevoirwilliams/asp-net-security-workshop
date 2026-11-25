# Security Review Checklist

## Request and input handling
| Review area | Expected protection | Observed state | Risk note | Priority | Next action |
|---|---|---|---|---|---|
| Form model validation | Data annotations + ModelState enforced server-side | | | | |
| File upload validation | Extension, size, and deep content checks | | | | |

## Authentication and authorization
| Review area | Expected protection | Observed state | Risk note | Priority | Next action |
|---|---|---|---|---|---|
| Role-based access checks | Reviewer/Admin endpoints require explicit policy | | | | |
| Ownership controls | Employees restricted to own requests and attachments | | | | |

## Sensitive data protection
| Review area | Expected protection | Observed state | Risk note | Priority | Next action |
|---|---|---|---|---|---|
| Attachment handling | Files stored outside wwwroot and not directly served | | | | |
| Logging data exposure | Sensitive workflow data is minimized in logs | | | | |

## Configuration and environment safety
| Review area | Expected protection | Observed state | Risk note | Priority | Next action |
|---|---|---|---|---|---|
| Secret management | Secrets stored in Secret Manager/env vars/secret store | | | | |
| Environment-specific behavior | Development-only behavior disabled in production | | | | |

## Error handling and logging
| Review area | Expected protection | Observed state | Risk note | Priority | Next action |
|---|---|---|---|---|---|
| Error detail control | Production responses are generic | | | | |
| Structured logging | Useful context without sensitive payloads | | | | |

## Release readiness
| Review area | Expected protection | Observed state | Risk note | Priority | Next action |
|---|---|---|---|---|---|
| Finding triage | Findings prioritized and ownership assigned | | | | |
| Validation evidence | Remediation validated before release | | | | |
