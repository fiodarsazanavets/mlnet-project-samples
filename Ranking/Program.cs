using Microsoft.ML.Data;
using Microsoft.ML;
using Ranking;

var context = new MLContext();

var data = context.Data.LoadFromTextFile<Input>("relevance-ranking.csv", separatorChar: ',');

var split = context.Data.TrainTestSplit(data, testFraction: 0.2);

var secondSplit = context.Data.TrainTestSplit(split.TestSet);

var sampleInput = secondSplit.TestSet;

var rankingPipeline = context.Transforms.Conversion.MapValueToKey("Label")
    .Append(context.Transforms.Conversion.Hash("GroupId", "GroupId"))
    .Append(context.Ranking.Trainers.LightGbm());

var model = rankingPipeline.Fit(split.TrainSet);

var predictions = model.Transform(split.TestSet);

var options = new RankingEvaluatorOptions
{
    DcgTruncationLevel = 5
};

var metrics = context.Ranking.Evaluate(predictions, options);

var ndcg = metrics.NormalizedDiscountedCumulativeGains.Average();

Console.WriteLine($"Normalized Discounted Cumulative Gain: {ndcg}");
Console.Write(Environment.NewLine);

var batchPredictions = model.Transform(sampleInput);

var newPredictions = context.Data.CreateEnumerable<Output>(batchPredictions, reuseRowObject: false);

Console.WriteLine("Scores:");
foreach (var prediction in newPredictions)
{
    Console.WriteLine($"{prediction.Score}");
}