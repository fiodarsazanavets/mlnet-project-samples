using Microsoft.ML.Data;

namespace TechSupportBotTrainer.Data;

public class TicketData
{
    [LoadColumn(0)]
    public string Text { get; set; } = string.Empty;

    [LoadColumn(1)]
    public string Label { get; set; } = string.Empty;
}