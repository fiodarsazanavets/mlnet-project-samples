using Microsoft.ML.Data;

namespace TechSupportBotTrainer.Data;

public class SimilarityData
{
    [LoadColumn(0)]
    public string Sentence1 { get; set; } = string.Empty;

    [LoadColumn(1)]
    public string Sentence2 { get; set; } = string.Empty;

    [LoadColumn(2)]
    public float Label { get; set; }
}
