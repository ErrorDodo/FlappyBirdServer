using Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Common.Models;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScoreController : ControllerBase
{
    private readonly ILogger<ScoreController> _logger;
    private readonly IScoreManagerService _scoreService;

    public ScoreController(ILogger<ScoreController> logger, IScoreManagerService scoreService)
    {
        _logger = logger;
        _scoreService = scoreService;
        _logger.LogInformation("ScoreController instantiated.");
    }
        
    [HttpGet]
    public ActionResult<IEnumerable<HighScoreModel>> GetTopScores()
    {
        _logger.LogInformation("GET /Score called to retrieve top scores.");
        try
        {
            _logger.LogDebug("Calling _scoreService.GetScores()");
            var scores = _scoreService.GetScores();
            _logger.LogDebug("Retrieved {ScoreCount} scores from ScoreManagerService.", scores.Count());

            var topScores = scores
                .OrderByDescending(s => s.Score)
                .Take(5)
                .ToList();
            
            _logger.LogInformation("Returning {TopScoreCount} top scores.", topScores.Count);
            return Ok(topScores);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving scores in GET /Score.");
            return StatusCode(500, "Internal server error");
        }
    }
        
    [HttpPost]
    public ActionResult AddScore([FromForm] HighScoreModel newScore)
    {
        _logger.LogInformation("POST /Score called to add a new score.");
        
        if (newScore == null)
        {
            _logger.LogWarning("POST /Score received null newScore.");
            return BadRequest("Score data is required.");
        }
        
        _logger.LogDebug("Received new score for {Name} with value {Score}.", newScore.Name, newScore.Score);
        
        if (string.IsNullOrWhiteSpace(newScore.Name))
        {
            _logger.LogWarning("POST /Score detected empty name in newScore.");
            return BadRequest("Name cannot be empty.");
        }
        if (newScore.Score < 0)
        {
            _logger.LogWarning("POST /Score detected negative score in newScore.");
            return BadRequest("Score must be a non-negative number.");
        }

        try
        {
            _logger.LogDebug("Calling _scoreService.SaveScore() for new score.");
            _scoreService.SaveScore(newScore);
            _logger.LogInformation("Score for {Name} saved successfully.", newScore.Name);
            return Ok("Score saved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving score in POST /Score.");
            return StatusCode(500, "Internal server error");
        }
    }
}
