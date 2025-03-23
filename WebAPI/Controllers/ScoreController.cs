using Microsoft.AspNetCore.Mvc;
using Common.Services;
using Common.Models;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ScoreController : ControllerBase
{
    private readonly ILogger<ScoreController> _logger;
    private readonly ScoreManagerService _scoreService;

    public ScoreController(ILogger<ScoreController> logger, ScoreManagerService scoreService)
    {
        _logger = logger;
        _scoreService = scoreService;
    }
        
    [HttpGet]
    public ActionResult<IEnumerable<HighScoreModel>> GetTopScores()
    {
        try
        {
            var scores = _scoreService.GetScores();
            var topScores = scores
                .OrderByDescending(s => s.Score)
                .Take(5)
                .ToList();

            return Ok(topScores);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving scores");
            return StatusCode(500, "Internal server error");
        }
    }
        
    [HttpPost]
    public ActionResult AddScore([FromBody] HighScoreModel newScore)
    {
        if (newScore == null)
        {
            return BadRequest("Score data is required.");
        }
        if (string.IsNullOrWhiteSpace(newScore.Name))
        {
            return BadRequest("Name cannot be empty.");
        }
        if (newScore.Score < 0)
        {
            return BadRequest("Score must be a non-negative number.");
        }

        try
        {
            _scoreService.SaveScore(newScore);
            return Ok("Score saved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving score");
            return StatusCode(500, "Internal server error");
        }
    }
}