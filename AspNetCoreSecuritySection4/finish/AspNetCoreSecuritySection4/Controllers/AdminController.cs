using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSecuritySection4.Controllers;

[Authorize(Roles = "Administrator")]
public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [Authorize(Policy = "DemoPolicy")]
    public IActionResult PolicyDemo()
    {
        return View();
    }
}
