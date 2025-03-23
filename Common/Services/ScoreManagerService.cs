using Newtonsoft.Json;
using Common.Models;

namespace Common.Services;

public class ScoreManagerService
{
    private readonly string _filePath;
    private readonly object _fileLock = new();

    public ScoreManagerService()
    {
        _filePath = Path.Combine(Directory.GetCurrentDirectory(), "scores.json");
    }
    
    public List<HighScoreModel> GetScores()
    {
        lock (_fileLock)
        {
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
                return [];
            }

            try
            {
                var json = File.ReadAllText(_filePath);
                var scores = JsonConvert.DeserializeObject<List<HighScoreModel>>(json);
                return scores ?? [];
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to load scores", ex);
            }
        }
    }
    
    public void SaveScore(HighScoreModel score)
    {
        lock (_fileLock)
        {
            var scores = GetScores();

            scores.Add(score);
                
            try
            {
                var json = JsonConvert.SerializeObject(scores, Formatting.Indented);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to save score", ex);
            }
        }
    }
}
