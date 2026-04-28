namespace TechSupportBotTrainer.Data;

public sealed class NamedEntityPrediction
{
    public string Sentence { get; set; } = string.Empty;
    public string[] OriginalLabel { get; set; } = [];
    public string[] PredictedLabel { get; set; } = [];
}