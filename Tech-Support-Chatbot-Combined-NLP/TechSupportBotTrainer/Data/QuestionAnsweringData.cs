using Microsoft.ML.Data;

namespace TechSupportBotTrainer.Data;

public sealed class QuestionAnsweringData
{
    [LoadColumn(0)]
    public string Context { get; set; } = string.Empty;

    [LoadColumn(1)]
    public string Question { get; set; } = string.Empty;

    [LoadColumn(2)]
    public string TrainingAnswer { get; set; } = string.Empty;

    [LoadColumn(3)]
    public int AnswerIndex { get; set; }
}