# ASP.NET Core Security Workshop — Sample Projects

Use these sample applications and review materials to follow the hands-on lessons. Find your topic below, open the matching project, and use the checkpoints to decide when to move on. Run the training apps locally with test data.

## Set up your workspace

1. Install the [.NET 11 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/11.0) and a C# editor. The SDK requirement starts at 11.0.100. The videos may show .NET 10 or earlier tooling. Use .NET 11 with this repository.
2. Select **Code → Download ZIP** and extract it, or run `git clone https://github.com/trevoirwilliams/asp-net-security-workshop.git`.
3. From the extracted repository, run `dotnet --version`. It should report a stable `11.0.x` SDK. Open the solution in your editor or use a terminal in the folder containing the `.csproj` you want to run.
4. Run `dotnet restore`, then `dotnet run --launch-profile https`. Open the localhost address printed in the terminal. On Windows or macOS, run `dotnet dev-certs https --trust` if you need to trust the local development certificate. See the [development certificate guide](https://learn.microsoft.com/dotnet/core/tools/dotnet-dev-certs) for your platform.

Use `start` to follow a demonstration and `finish` to compare your work. Start and finish are independent copies. Stop one before running the other because they can use the same ports. Keep the SQLite sample files that come with the download. Section 3, Section 6, and SecureReviewPortal apply their existing migrations on startup. The JWT Web API initializes its local database when it starts.

## Find your project

| Lesson topic | Start here | Compare your work |
| --- | --- | --- |
| 3 · Input validation, safe data handling, and uploads | [Starter solution](AspNetCoreSecuritySection3/start/AspNetCoreSecuritySection3.sln) | [Finished solution](AspNetCoreSecuritySection3/finish/AspNetCoreSecuritySection3.sln) |
| 4 · Identity, authorization, and JWT | Create the Identity app using the **Individual Accounts** option in the walkthrough. The JWT demo has a separate API project. | [Identity web app](AspNetCoreSecuritySection4/finish/AspNetCoreSecuritySection4/AspNetCoreSecuritySection4.csproj) · [JWT Web API](AspNetCoreSecuritySection4/finish/AspNetCoreApiSecuritySection4/AspNetCoreApiSecuritySection4.csproj) · [Combined solution](AspNetCoreSecuritySection4/finish/AspNetCoreSecuritySection4.slnx) |
| 5 · Data protection, secrets, and cryptography | [Secure Notes starter solution](AspNetCoreSecuritySection5/start/AspNetCoreSecuritySection5.sln) | [Finished solution](AspNetCoreSecuritySection5/finish/AspNetCoreSecuritySection5.sln) |
| 6 · Threat modeling and STRIDE | [Task management starter solution](AspNetCoreSecuritySection6/start/AspNetCoreSecuritySection6.sln) · [Starter guide](AspNetCoreSecuritySection6/start/AspNetCoreSecuritySection6/README.md) | [Finished solution](AspNetCoreSecuritySection6/finish/AspNetCoreSecuritySection6.sln) · [Finished guide](AspNetCoreSecuritySection6/finish/AspNetCoreSecuritySection6/README.md) |
| 7 · Security code review and release readiness | [SecureReviewPortal starter solution](AspNetCoreSecuritySection7/start/AspNetCoreSecuritySection7.slnx) · [Review templates](AspNetCoreSecuritySection7/start/SecureReviewPortal/docs) | [Finished solution](AspNetCoreSecuritySection7/finish/AspNetCoreSecuritySection7.slnx) · [Completed review materials](AspNetCoreSecuritySection7/finish/SecureReviewPortal/docs) |

## Checkpoints

### Input validation, safe data handling, and uploads

- Run the starter and open a sample form before editing. After adding server-side validation, verify that valid input is accepted and a harmless invalid submission receives a validation response. Check the MVC form and the API separately.
- After the upload and request-control lessons, verify the allowed upload path and the rejection of a file outside your policy. Confirm where accepted files are stored. Explain which response comes from input validation, which comes from a business rule, and which comes from rate limiting.

### Identity, authorization, and JWT

- Run the Identity web app and JWT Web API as separate projects. Establish that sign-in works before changing cookie and session settings. Use local learner accounts to check a permitted action and a denied action under the policy you configured.
- In the JWT Web API, verify that a protected endpoint rejects a request without credentials and accepts a valid token for an authorized user. Record the observed status codes. Treat the completed projects as references for the walkthrough. This section ships in `finish`.

### Data protection, secrets, and cryptography

- Open the Secure Notes starter before the first Data Protection demo. Verify that a valid protected value can be recovered for its intended purpose. Write down which settings belong in local secret storage and which belong with the Data Protection key ring.
- Compare one-way password hashing, integrity verification, and reversible encryption in the finished sample. Use throwaway values for the examples and keep secret values out of your notes and commits.

### Threat modeling and STRIDE

- Run the starter task-management app and choose one workflow. Mark its assets, entry points, data stores, and trust boundaries on a simple data-flow diagram.
- Pick one STRIDE finding and turn it into a requirement with an observable pass condition. Open the finished app during design validation and record the evidence that supports your conclusion.

### Security code review and release readiness

1. Start with the [security review checklist](AspNetCoreSecuritySection7/start/SecureReviewPortal/docs/security-review-checklist.md) and [manual review notes template](AspNetCoreSecuritySection7/start/SecureReviewPortal/docs/manual-review-notes-template.md). Record which control you inspected and what evidence supports the finding.
2. Verify any AI-assisted suggestions against the application and enter confirmed items in the [triage template](AspNetCoreSecuritySection7/start/SecureReviewPortal/docs/findings-triage-template.md).
3. Write your recommendation using the [release readiness template](AspNetCoreSecuritySection7/start/SecureReviewPortal/docs/release-readiness-summary-template.md). Compare it with the [validation results](AspNetCoreSecuritySection7/finish/SecureReviewPortal/docs/validation-results.md) and [completed release summary](AspNetCoreSecuritySection7/finish/SecureReviewPortal/docs/release-readiness-summary.md) after you finish.

## Troubleshooting

- **The SDK cannot be found.** Run `dotnet --list-sdks` and check that a stable .NET 11 SDK is installed. Reopen your terminal or editor after installation.
- **The port is in use.** Stop the other workshop app, then restart the project and use the address printed in the terminal.
- **A database error appears.** Check that you are running from the folder containing the intended `.csproj` and that the extracted sample files are present. Try a fresh extraction in a separate folder to get a clean copy of the sample data.
- **The code looks different from the video.** Check your topic and whether you opened `start` or `finish`. The completed folders show the end of a section. Use the walkthrough for the changes made in each lesson.
