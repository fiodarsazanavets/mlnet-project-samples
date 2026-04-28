using Microsoft.ML;
using Microsoft.ML.TorchSharp;
using TechSupportBotTrainer.Data;

namespace TechSupportBotTrainer.Trainers;

public static class TextClassificationTrainer
{
    public static void TrainTextClassificationModel(this MLContext ml, string baseTrainingDataPath, string baseModelsPath)
    {
        var data = ml.Data.LoadFromTextFile<TicketData>(
            path: $"{baseTrainingDataPath}{Path.DirectorySeparatorChar}stage1_text_classification.csv",
            hasHeader: true,
            separatorChar: ',');

        var split = ml.Data.TrainTestSplit(data, testFraction: 0.2);

        var pipeline =
            ml.Transforms.Conversion.MapValueToKey("Label")
              .Append(ml.MulticlassClassification.Trainers.TextClassification(
                  labelColumnName: "Label",
                  scoreColumnName: "Score",
                  outputColumnName: "PredictedLabel",
                  sentence1ColumnName: "Text",
                  maxEpochs: 20))
              .Append(ml.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        Console.WriteLine("Training text classification model...");
        var model = pipeline.Fit(split.TrainSet);

        Console.WriteLine("Evaluating text classification model...");
        var predictions = model.Transform(split.TestSet);
        var metrics = ml.MulticlassClassification.Evaluate(predictions);

        Console.WriteLine($"Text Classification Micro Accuracy : {metrics.MicroAccuracy:F3}");
        Console.WriteLine($"Text Classification Macro Accuracy: {metrics.MacroAccuracy:F3}");
        Console.WriteLine();

        string fullModelPath = $"{baseModelsPath}{Path.DirectorySeparatorChar}TextClassification.mlnet";

        using var fs = File.Create(fullModelPath);
        DataViewSchema dataViewSchema = data.Schema;
        ml.Model.Save(model, dataViewSchema, fs);
        Console.WriteLine($"Text Classification model has been saved at: {fullModelPath}");
        Console.WriteLine();
    }
}
