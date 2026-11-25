namespace SecureReviewPortal.Models;

public enum DataClassification
{
    Public = 0,
    Internal = 1,
    Confidential = 2,
    Restricted = 3
}

public enum SecurityReviewRequestStatus
{
    Draft = 0,
    Submitted = 1,
    InReview = 2,
    RemediationRequired = 3,
    Approved = 4,
    Rejected = 5
}

public enum FindingSeverity
{
    Critical = 0,
    High = 1,
    Medium = 2,
    Low = 3,
    Informational = 4
}

public enum FindingSource
{
    ManualReview = 0,
    AiAssistedReview = 1,
    Validation = 2
}

public enum FindingStatus
{
    Open = 0,
    NeedsVerification = 1,
    Remediated = 2,
    AcceptedRisk = 3,
    Deferred = 4
}
