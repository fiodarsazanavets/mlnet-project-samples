using Microsoft.Extensions.Configuration;
using Microsoft.ML;
using TechSupportBot.Core.Abstractions;
using TechSupportBot.Core.Data;
using TechSupportBot.Core.Helpers;

namespace TechSupportBot.Core.Services;

public class TicketClassificationService : ITicketClassificationService
{
    private readonly MLContext _mlContext = new();
    private readonly string _fullModelPath;
    private readonly Lazy<ITransformer> _model;

    public TicketClassificationService(IConfiguration configuration)
    {
        _fullModelPath = configuration["ModelPaths:TextClassification"]
            ?? "TextClassification.mlnet";

        _model = new Lazy<ITransformer>(() => ModelLoaderHelper.LoadModel(_mlContext, _fullModelPath), isThreadSafe: true);
    }

    public TicketPrediction Predict(TicketData input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var predictionEngine =
            _mlContext.Model.CreatePredictionEngine<TicketData, TicketPrediction>(_model.Value);

        return predictionEngine.Predict(input);
    }
}