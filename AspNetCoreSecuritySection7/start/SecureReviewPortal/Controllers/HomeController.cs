using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SecureReviewPortal.Models;

namespace SecureReviewPortal.Controllers;

public class HomeController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public HomeController(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        var exposeDetails = _configuration.GetValue<bool>("ErrorHandling:ExposeExceptionDetails");

        var model = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            Path = feature?.Path,
            Message = feature?.Error.Message,
            ExceptionDetails = exposeDetails && _environment.IsDevelopment() ? feature?.Error.ToString() : null
        };

        return View(model);
    }
}
