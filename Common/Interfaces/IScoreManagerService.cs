using Common.Models;

namespace Common.Interfaces;

public interface IScoreManagerService
{
    List<HighScoreModel> GetScores();
    void SaveScore(HighScoreModel score);
}