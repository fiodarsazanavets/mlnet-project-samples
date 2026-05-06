using TechSupportBot.Core.Data;

namespace TechSupportBot.Core.Abstractions;

public interface ITicketClassificationService
{
    TicketPrediction Predict(TicketData input);
}
