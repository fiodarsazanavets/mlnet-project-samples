using Microsoft.ML.Data;

namespace TechSupportBotTrainer.Data;

public sealed class QuestionAnsweringPrediction
{
    public string Context { get; set; } = string.Empty;

    public string Question { get; set; } = string.Empty;

    public string TrainingAnswer { get; set; } = string.Empty;

    public int AnswerIndex { get; set; }

    public VBuffer<ReadOnlyMemory<char>> Answer { get; set; }

    public VBuffer<float> Score { get; set; }
}