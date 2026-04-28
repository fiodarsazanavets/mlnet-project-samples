using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.TorchSharp;
using TechSupportBotTrainer.Data;

namespace TechSupportBotTrainer.Trainers;

public static class QuestionAnsweringTrainer
{
    public static void TrainQuestionAnsweringModel(
        this MLContext ml,
        string baseTrainingDataPath,
        string baseModelsPath)
    {
        var data = ml.Data.LoadFromTextFile<QuestionAnsweringData>(
            path: Path.Combine(baseTrainingDataPath, "stage4_question_answering.csv"),
            hasHeader: true,
            separatorChar: ',',
            allowQuoting: true);

        var split = ml.Data.TrainTestSplit(data, testFraction: 0.2);

        var pipeline =
            ml.MulticlassClassification.Trainers.QuestionAnswer(
                contextColumnName: "Context",
                questionColumnName: "Question",
                trainingAnswerColumnName: "TrainingAnswer",
                answerIndexColumnName: "AnswerIndex",
                predictedAnswerColumnName: "Answer",
                scoreColumnName: "Score",
                topK: 3,
                batchSize: 4,
                maxEpochs: 20);

        Console.WriteLine("Training question answering model...");
        var model = pipeline.Fit(split.TrainSet);

        Console.WriteLine("Evaluating question answering model...");
        var predictions = model.Transform(split.TestSet);

        var predictionRows = ml.Data
            .CreateEnumerable<QuestionAnsweringPrediction>(
                predictions,
                reuseRowObject: false)
            .ToList();

        var top1ExactMatch = CalculateTop1ExactMatch(predictionRows);
        var topKExactMatch = CalculateTopKExactMatch(predictionRows);

        Console.WriteLine($"Top-1 Exact Match for Question Answering: {top1ExactMatch:F3}");
        Console.WriteLine($"Top-K Exact Match for Question Answering: {topKExactMatch:F3}");
        Console.WriteLine();

        string fullModelPath = Path.Combine(baseModelsPath, "QuestionAnswering.mlnet");

        using var fs = File.Create(fullModelPath);
        DataViewSchema dataViewSchema = data.Schema;
        ml.Model.Save(model, dataViewSchema, fs);

        Console.WriteLine($"Question Answering model has been saved at: {fullModelPath}");
        Console.WriteLine();
    }

    private static double CalculateTop1ExactMatch(
        IEnumerable<QuestionAnsweringPrediction> predictions)
    {
        var correct = 0;
        var total = 0;

        foreach (var prediction in predictions)
        {
            var expectedAnswer = NormalizeAnswer(prediction.TrainingAnswer);
            var predictedAnswers = GetPredictedAnswers(prediction.Answer);

            if (predictedAnswers.Count > 0 &&
                NormalizeAnswer(predictedAnswers[0]) == expectedAnswer)
            {
                correct++;
            }

            total++;
        }

        return total == 0 ? 0 : (double)correct / total;
    }

    private static double CalculateTopKExactMatch(
        IEnumerable<QuestionAnsweringPrediction> predictions)
    {
        var correct = 0;
        var total = 0;

        foreach (var prediction in predictions)
        {
            var expectedAnswer = NormalizeAnswer(prediction.TrainingAnswer);
            var predictedAnswers = GetPredictedAnswers(prediction.Answer);

            if (predictedAnswers.Any(answer => NormalizeAnswer(answer) == expectedAnswer))
            {
                correct++;
            }

            total++;
        }

        return total == 0 ? 0 : (double)correct / total;
    }

    private static List<string> GetPredictedAnswers(
        VBuffer<ReadOnlyMemory<char>> answerBuffer)
    {
        var answers = new List<string>();

        foreach (var answer in answerBuffer.GetValues())
        {
            answers.Add(answer.ToString());
        }

        return answers;
    }

    private static string NormalizeAnswer(string? answer)
    {
        return string.IsNullOrWhiteSpace(answer)
            ? string.Empty
            : answer.Trim().ToLowerInvariant();
    }
}
