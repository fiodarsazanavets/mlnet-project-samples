using Microsoft.ML;

namespace ErrorCategorization;

public static class ErrorPatternMatcher
{
    public static Output PredictErrorCategory(Input input)
    {
        var mlContext = new MLContext();
        ITransformer model = mlContext.Model.Load(Constants.ModelName, out var schema);

        PredictionEngine<Input, Output> predictor =
            mlContext.Model.CreatePredictionEngine<Input, Output>(model);

        return predictor.Predict(input);
    }
}
