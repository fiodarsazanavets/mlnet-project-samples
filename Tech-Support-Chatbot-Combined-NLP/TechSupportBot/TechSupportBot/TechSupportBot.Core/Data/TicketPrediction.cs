namespace TechSupportBot.Core.Data;

public class TicketPrediction
{
    public string PredictedLabel { get; set; } = "";
    public float[] Score { get; set; } = Array.Empty<float>();
}
