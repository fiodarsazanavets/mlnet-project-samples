using Microsoft.ML;
using Microsoft.ML.TorchSharp;
using TechSupportBotTrainer.Data;

namespace TechSupportBotTrainer.Trainers;

public static class NamedEntityRecognitionTrainer
{
    public static void TrainNamedEntityRecognitionModel(
        this MLContext ml,
        string baseTrainingDataPath,
        string baseModelsPath)
    {
        string trainingDataPath = Path.Combine(
            baseTrainingDataPath,
            "stage3_ner_training.tsv");

        string keyInfoPath = Path.Combine(
            baseTrainingDataPath,
            "stage3_ner_keyinfo.txt");

        var trainingRows = LoadNamedEntityData(trainingDataPath);

        var data = ml.Data.LoadFromEnumerable(trainingRows);

        var split = ml.Data.TrainTestSplit(data, testFraction: 0.2);

        var labels = LoadEntityLabels(keyInfoPath);

        var labelKeyData = ml.Data.LoadFromEnumerable(
            labels.Select(label => new NamedEntityLabel { Label = label }));

        var pipeline =
            ml.Transforms.Conversion.MapValueToKey(
                    outputColumnName: "Label",
                    inputColumnName: "Label",
                    keyData: labelKeyData)
              .Append(ml.MulticlassClassification.Trainers.NamedEntityRecognition(
                    labelColumnName: "Label",
                    outputColumnName: "PredictedLabel",
                    sentence1ColumnName: "Sentence",
                    batchSize: 8,
                    maxEpochs: 20))
              .Append(ml.Transforms.Conversion.MapKeyToValue(
                    outputColumnName: "PredictedLabel",
                    inputColumnName: "PredictedLabel"));

        Console.WriteLine("Training named entity recognition model...");
        var model = pipeline.Fit(split.TrainSet);

        Console.WriteLine("Evaluating named entity recognition model...");
        var predictions = model.Transform(split.TestSet);

        var predictionRows = ml.Data
            .CreateEnumerable<NamedEntityPrediction>(
                predictions,
                reuseRowObject: false)
            .ToList();

        var tokenAccuracy = CalculateTokenAccuracy(predictionRows);

        Console.WriteLine($"Token Accuracy for Named Entity Recognition: {tokenAccuracy:F3}");
        Console.WriteLine();

        string fullModelPath = Path.Combine(baseModelsPath, "NamedEntityRecognition.mlnet");

        using var fs = File.Create(fullModelPath);
        ml.Model.Save(model, data.Schema, fs);

        Console.WriteLine($"Named Entity Recognition model has been saved at: {fullModelPath}");
        Console.WriteLine();
    }

    private static List<NamedEntityData> LoadNamedEntityData(string path)
    {
        var rows = new List<NamedEntityData>();

        foreach (var line in File.ReadLines(path).Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var columns = line.Split('\t');

            if (columns.Length < 2)
                throw new InvalidDataException($"Invalid NER training row: {line}");

            var sentence = columns[0].Trim();

            var tokens = sentence.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

            var allLabels = columns
                .Skip(1)
                .Select(label => label.Trim())
                .Where(label => !string.IsNullOrWhiteSpace(label))
                .ToArray();

            if (allLabels.Length < tokens.Length)
            {
                throw new InvalidDataException(
                    $"Not enough labels for sentence '{sentence}'. " +
                    $"Tokens: {tokens.Length}, labels: {allLabels.Length}");
            }

            var labels = allLabels
                .Take(tokens.Length)
                .ToArray();

            rows.Add(new NamedEntityData
            {
                Sentence = sentence,
                Label = labels,
                OriginalLabel = labels
            });
        }

        return rows;
    }

    private static string[] LoadEntityLabels(string path)
    {
        var labels = File.ReadLines(path)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();

        if (!labels.Contains("O"))
            labels.Insert(0, "O");

        return labels.Distinct().ToArray();
    }

    private static double CalculateTokenAccuracy(IEnumerable<NamedEntityPrediction> predictions)
    {
        var correct = 0;
        var total = 0;

        foreach (var prediction in predictions)
        {
            var actualLabels = prediction.OriginalLabel ?? [];
            var predictedLabels = prediction.PredictedLabel ?? [];

            var length = Math.Min(actualLabels.Length, predictedLabels.Length);

            for (var i = 0; i < length; i++)
            {
                if (actualLabels[i] == predictedLabels[i])
                    correct++;

                total++;
            }
        }

        return total == 0 ? 0 : (double)correct / total;
    }
}
