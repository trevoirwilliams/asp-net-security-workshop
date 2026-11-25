using AspNetCoreSecuritySection3.Data;
using AspNetCoreSecuritySection3.Models;
using AspNetCoreSecuritySection3.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSecuritySection3.Controllers.Api;

[Route("api/[controller]")]
public class SurveysController : ControllerBase
{
    private readonly AppDbContext _db;

    public SurveysController(AppDbContext db) => _db = db;

    // POST /api/surveys
    // COURSE NOTE: strengthen validation and ModelState handling in Video 3.3
    // COURSE NOTE: add rate limiting in Video 3.6
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ApiSurveyRequest request)
    {
        if (request is null)
            return BadRequest("Invalid JSON payload.");

        var survey = new SurveySubmission
        {
            Name = request.Name,
            Email = request.Email,
            Score = request.Score,
            Comment = request.Comment,
            SubmittedAtUtc = DateTime.UtcNow
        };

        _db.SurveySubmissions.Add(survey);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Post), new { id = survey.Id }, new
        {
            survey.Id,
            survey.Name,
            survey.Email,
            survey.Score,
            survey.SubmittedAtUtc
        });
    }
}
