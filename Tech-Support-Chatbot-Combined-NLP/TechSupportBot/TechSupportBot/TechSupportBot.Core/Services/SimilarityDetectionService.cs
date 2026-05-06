using Microsoft.Extensions.Configuration;
using Microsoft.ML;
using TechSupportBot.Core.Abstractions;
using TechSupportBot.Core.Data;
using TechSupportBot.Core.Helpers;

namespace TechSupportBot.Core.Services;

public class SimilarityDetectionService : ISimilarityDetectionService
{
    private readonly MLContext _mlContext = new();
    private readonly string _fullModelPath;
    private readonly Lazy<ITransformer> _model;

    public SimilarityDetectionService(IConfiguration configuration)
    {
        _fullModelPath = configuration["ModelPaths:SentenceSimilarity"]
            ?? "SentenceSimilarity.mlnet";

        _model = new Lazy<ITransformer>(() => ModelLoaderHelper.LoadModel(_mlContext, _fullModelPath), isThreadSafe: true);
    }

    public SimilarityPrediction Predict(SimilarityData input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var predictionEngine =
            _mlContext.Model.CreatePredictionEngine<SimilarityData, SimilarityPrediction>(_model.Value);

        return predictionEngine.Predict(input);
    }
}