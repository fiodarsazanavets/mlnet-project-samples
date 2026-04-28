using Microsoft.ML;
using Microsoft.ML.TorchSharp;
using TechSupportBotTrainer.Data;

namespace TechSupportBotTrainer.Trainers;

public static class SentenceSimilarityTrainer
{
    public static void TrainSentenceSimilarityModel(this MLContext ml, string baseTrainingDataPath, string baseModelsPath)
    {
        var data = ml.Data.LoadFromTextFile<SimilarityData>(
            path: $"{baseTrainingDataPath}{Path.DirectorySeparatorChar}/stage2_sentence_similarity.csv",
            hasHeader: true,
            separatorChar: ',');

        var split = ml.Data.TrainTestSplit(data, testFraction: 0.2);

        var pipeline =
            ml.Regression.Trainers.SentenceSimilarity(
                labelColumnName: "Label",
                scoreColumnName: "Score",
                sentence1ColumnName: "Sentence1",
                sentence2ColumnName: "Sentence2",
                maxEpochs: 20);

        Console.WriteLine("Training sentence similarity model...");
        var model = pipeline.Fit(split.TrainSet);

        Console.WriteLine("Evaluating text classification model...");
        var predictions = model.Transform(split.TestSet);
        var metrics = ml.Regression.Evaluate(predictions);

        Console.WriteLine($"R-Squared for Sentence Similarity: {metrics.RSquared:F3}");
        Console.WriteLine($"RMSE for Sentence Similarity: {metrics.RootMeanSquaredError:F3}");

        Console.WriteLine();

        string fullModelPath = $"{baseModelsPath}{Path.DirectorySeparatorChar}SentenceSimilarity.mlnet";

        using var fs = File.Create(fullModelPath);
        DataViewSchema dataViewSchema = data.Schema;
        ml.Model.Save(model, dataViewSchema, fs);
        Console.WriteLine($"Sentence Similarity model has been saved at: {fullModelPath}");
        Console.WriteLine();
    }
}
