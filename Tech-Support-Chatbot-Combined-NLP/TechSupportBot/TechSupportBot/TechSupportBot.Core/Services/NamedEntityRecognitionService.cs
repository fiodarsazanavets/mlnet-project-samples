using Microsoft.Extensions.Configuration;
using Microsoft.ML;
using TechSupportBot.Core.Abstractions;
using TechSupportBot.Core.Data;
using TechSupportBot.Core.Helpers;

namespace TechSupportBot.Core.Services;

public class NamedEntityRecognitionService : INamedEntityRecognitionService
{
    private readonly MLContext _mlContext = new();
    private readonly string _fullModelPath;
    private readonly Lazy<ITransformer> _model;

    public NamedEntityRecognitionService(IConfiguration configuration)
    {
        _fullModelPath = configuration["ModelPaths:NamedEntityRecognition"]
            ?? "NamedEntityRecognition.mlnet";

        _model = new Lazy<ITransformer>(() => ModelLoaderHelper.LoadModel(_mlContext, _fullModelPath), isThreadSafe: true);
    }

    public NamedEntityPrediction Predict(NamedEntityData input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var predictionEngine =
            _mlContext.Model.CreatePredictionEngine<NamedEntityData, NamedEntityPrediction>(_model.Value);

        return predictionEngine.Predict(input);
    }
}