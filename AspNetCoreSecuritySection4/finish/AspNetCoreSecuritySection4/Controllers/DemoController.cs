using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSecuritySection4.Controllers;

[Authorize]
public class DemoController : Controller
{
    public IActionResult SessionState()
    {
        const string key = "LastSecureVisitUtc";

        var previousVisit = HttpContext.Session.GetString(key);
        var currentVisit = DateTime.UtcNow.ToString("O");

        HttpContext.Session.SetString(key, currentVisit);

        ViewData["PreviousVisit"] = previousVisit ?? "First secure visit in this session";
        ViewData["CurrentVisit"] = currentVisit;

        return View();
    }

}
