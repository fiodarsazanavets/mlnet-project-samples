using Microsoft.ML;

namespace ErrorCategorization;

public class ErrorPatternDetector
{
    public static void DetectPatterns(string inputFilePath)
    {
        MLContext mlContext = new(seed: 0);

        IDataView? dataView = mlContext.Data.LoadFromTextFile<Input>(inputFilePath, hasHeader: false, separatorChar: '\t');

        string featuresColumnName = "Features";

        IEstimator<ITransformer> pipeline = mlContext.Transforms
            .Concatenate(featuresColumnName,
                "ErrorCode",
                "ErrorMessage")
            .Append(mlContext.Clustering.Trainers.KMeans(
                featuresColumnName, numberOfClusters: 3));

        ITransformer? model = pipeline.Fit(dataView);

        using FileStream fileStream = new(
            Constants.ModelName, FileMode.Create,
            FileAccess.Write, FileShare.Write);
        mlContext.Model.Save(model, dataView.Schema, fileStream);

        IEnumerable<Input> loadedInputData =
            mlContext.Data.CreateEnumerable<Input>(dataView, reuseRowObject: true);

        PredictionEngine<Input, Output> predictor =
            mlContext.Model.CreatePredictionEngine<Input, Output>(model);

        foreach (Input item in loadedInputData)
        {
            Output? prediction = predictor.Predict(item);
            ClusterData.AddItemToCluster(prediction.PredictedClusterId, item);
        }
    }
}
