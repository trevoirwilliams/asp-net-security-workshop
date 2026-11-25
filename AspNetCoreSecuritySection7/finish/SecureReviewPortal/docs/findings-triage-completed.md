# Findings Triage Template

## Triage Rules

| Severity | Meaning |
|---|---|
| Critical | Directly exploitable issue that may expose sensitive data, privileged actions, or system compromise with minimal effort. |
| High | Serious weakness affecting access control, sensitive workflows, or protected data, but may require specific conditions. |
| Medium | Meaningful weakness that should be remediated, but impact or exploitability is more limited. |
| Low | Hardening, consistency, maintainability, or release-readiness concern with limited immediate exposure. |
| Informational | Useful observation, positive control, or item that does not currently require remediation. |

## Status Values

| Status | Meaning |
|---|---|
| Confirmed | Evidence supports the finding. |
| Needs Verification | The finding may be valid but requires runtime or environment validation. |
| Duplicate | Same issue already captured under another finding. |
| Unsupported | Not enough evidence to carry forward. |
| Accepted Risk | Team accepts the risk for now with documented reasoning. |
| Deferred | Valid finding, but not required before this release. |


| Finding ID | Source | Title | Evidence | Severity | Status | Release blocker | Recommended action | Owner | Validation notes |
|---|---|---|---|---|---|---|---|---|---|
| SRP-001 | Manual + AI | Reviewer-facing action needs stronger role enforcement | `ReviewFindingsController.Create` appears to require authentication but not `Reviewer` or `Admin` role enforcement. | High | Confirmed | Yes | Add role or policy-based authorization for reviewer-facing finding actions. | Developer | Validate as Employee, Reviewer, and Admin after remediation. |
| SRP-002 | Manual + AI | File upload validation should be strengthened | `FileUploadService` checks file size and extension, but does not validate file signature or deeper file content. | Medium | Confirmed | No | Add stronger server-side file validation and keep uploads outside `wwwroot`. | Developer | Test allowed and disallowed files after remediation. |
| SRP-003 | Manual + AI | Logging may capture excessive business context | A review submission log includes business details such as title or justification. | Medium | Confirmed | No | Reduce log detail while preserving useful audit context such as user ID, request ID, and event type. | Developer | Confirm logs no longer include unnecessary business justification after remediation. |
| SRP-004 | Manual + AI | Development placeholder API key needs secret-management review | `appsettings.Development.json` contains `ExternalReviewService:ApiKey` with a fake development placeholder value. | Low | Needs Verification | No | Confirm real secrets are supplied through Secret Manager, environment variables, or a production secret store. | Developer / DevOps | Verify no real secrets are committed before release. |


## Duplicate Findings

| Duplicate Finding | Merged Into | Reason |
|---|---|---|
| AI finding about reviewer action authorization | SRP-001 | Same issue as manual access-control finding. |
| AI finding about upload extension validation | SRP-002 | Same issue as manual upload validation finding. |
| AI finding about detailed logging | SRP-003 | Same issue as manual logging finding. |

## Unsupported Findings


## Release Blocker Decision

| Finding ID | Release Blocker? | Reason |
|---|---|---|
| SRP-001 | Yes | Privileged reviewer workflow may be accessible without proper role enforcement. |
| SRP-002 | No | Upload validation should be strengthened, but storage is outside `wwwroot` and size/extension checks exist. |
| SRP-003 | No | Logging detail should be reduced, but this does not directly expose data to anonymous users. |
| SRP-004 | No | Placeholder value is fake and development-only, but secret handling must be verified. |
| SRP-005 | No | Production behavior needs validation but is not yet a confirmed unsafe behavior. |

## Remediation Plan

| Order | Finding ID | Remediation Goal | Planned Change | Validation Step |
|---|---|---|---|---|
| 1 | SRP-001 | Enforce reviewer/admin access | Add role or policy authorization to reviewer-facing finding actions. | Attempt access as Employee, Reviewer, and Admin. |
| 2 | SRP-003 | Reduce excessive logging detail | Remove business justification or detailed business fields from log messages. | Trigger workflow and inspect log output. |
| 3 | SRP-002 | Strengthen upload validation | Add stronger server-side file validation while keeping storage outside `wwwroot`. | Test allowed, disallowed, oversized, and mismatched files. |
| 4 | SRP-004 | Verify secret-management practice | Ensure real secrets are supplied through Secret Manager, environment variables, or production secret storage. | Search repository and confirm no real secrets are committed. |
| 5 | SRP-005 | Validate production error behavior | Run with production-like settings and confirm safe error response. | Trigger test error and inspect user response and logs. |


## Triage Summary

- Total findings carried forward: 5
- Release blockers: 1
- Confirmed findings: 3
- Findings needing verification: 2
- Unsupported AI findings rejected: 0
- First remediation priority: SRP-001