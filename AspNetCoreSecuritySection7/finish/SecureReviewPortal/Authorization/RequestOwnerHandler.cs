using Microsoft.AspNetCore.Authorization;
using SecureReviewPortal.Services;

namespace SecureReviewPortal.Authorization;

public class RequestOwnerHandler : AuthorizationHandler<RequestOwnerRequirement, int>
{
    private readonly SecurityReviewService _reviewService;

    public RequestOwnerHandler(SecurityReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RequestOwnerRequirement requirement,
        int resource)
    {
        var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return;
        }

        if (await _reviewService.UserOwnsRequestAsync(resource, userId))
        {
            context.Succeed(requirement);
        }
    }
}
