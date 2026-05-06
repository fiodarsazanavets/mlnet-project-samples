using Microsoft.Extensions.Configuration;
using Microsoft.ML;
using TechSupportBot.Core.Abstractions;
using TechSupportBot.Core.Data;
using TechSupportBot.Core.Helpers;

namespace TechSupportBot.Core.Services;

public class QuestionAnsweringService : IQuestionAnsweringService
{
    private readonly MLContext _mlContext = new();
    private readonly string _fullModelPath;
    private readonly Lazy<ITransformer> _model;

    public QuestionAnsweringService(IConfiguration configuration)
    {
        _fullModelPath = configuration["ModelPaths:NamedEntityRecognition"]
            ?? "QuestionAnswering.mlnet";

        _model = new Lazy<ITransformer>(() => ModelLoaderHelper.LoadModel(_mlContext, _fullModelPath), isThreadSafe: true);
    }

    public QuestionAnsweringPrediction Predict(QuestionAnsweringData input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var predictionEngine =
            _mlContext.Model.CreatePredictionEngine<QuestionAnsweringData, QuestionAnsweringPrediction>(_model.Value);

        return predictionEngine.Predict(input);
    }
}