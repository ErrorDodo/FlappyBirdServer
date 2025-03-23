using Newtonsoft.Json;

namespace Common.Models;

public class HighScoreModel
{
    [JsonProperty]
    public string Name { get; set; }
    [JsonProperty]
    public int Score { get; set; }
}