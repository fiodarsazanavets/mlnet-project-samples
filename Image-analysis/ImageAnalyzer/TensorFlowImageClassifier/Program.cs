using Microsoft.ML;
using TensorFlowImageClassifier;

string baseAssetsDirectory = "C:\\Temp\\ML.NET\\Data\\assets\\";

string imagesFolder = $"{baseAssetsDirectory}images";
string trainTagsTsv = $"{baseAssetsDirectory}images\\tags.tsv";
string testTagsTsv = $"{baseAssetsDirectory}images\\test-tags.tsv";
string predictSingleImage = $"{baseAssetsDirectory}images\\toaster3.jpg";
string inceptionTensorFlowModel = $"{baseAssetsDirectory}inception\\tensorflow_inception_graph.pb";

var mlContext = new MLContext();

var pipeline = mlContext.Transforms.LoadImages(
        outputColumnName: "input",
        imageFolder: imagesFolder,
        inputColumnName: nameof(InputData.ImagePath))
    .Append(mlContext.Transforms.ResizeImages(
        outputColumnName: "input",
        imageWidth: InceptionSettings.ImageWidth,
        imageHeight: InceptionSettings.ImageHeight,
        inputColumnName: "input"))
    .Append(mlContext.Transforms.ExtractPixels(
        outputColumnName: "input",
        interleavePixelColors: InceptionSettings.ChannelsLast,
        offsetImage: InceptionSettings.Mean))
    .Append(mlContext.Model.LoadTensorFlowModel(inceptionTensorFlowModel).ScoreTensorFlowModel(
        outputColumnNames:
            [
                "softmax2_pre_activation"
            ],
        inputColumnNames:
            [
                "input"
            ],
        addBatchDimensionInput: true))
    .Append(mlContext.Transforms.Conversion.MapValueToKey(
        outputColumnName: "LabelKey",
        inputColumnName: "Label"))
    .Append(mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(
        labelColumnName: "LabelKey",
        featureColumnName: "softmax2_pre_activation"))
    .Append(mlContext.Transforms.Conversion.MapKeyToValue(
        outputColumnName: "PredictedLabelValue",
        inputColumnName: "PredictedLabel"))
    .AppendCacheCheckpoint(mlContext);

var trainingData = mlContext.Data.LoadFromTextFile<InputData>(path: trainTagsTsv, hasHeader: false);
var model = pipeline.Fit(trainingData);

var testData = mlContext.Data.LoadFromTextFile<InputData>(path: testTagsTsv, hasHeader: false);
var predictions = model.Transform(testData);

var imagePredictionData = mlContext.Data.CreateEnumerable<Prediction>(predictions, true);

Console.WriteLine("Testing the model");

foreach (var imagePrediction in imagePredictionData)
{
    Console.WriteLine($"Image {Path.GetFileName(imagePrediction.ImagePath)} recognized as {imagePrediction.PredictedLabelValue} with confidence score of {imagePrediction.Score?.Max()}");
}

var imageData = new InputData()
{
    ImagePath = predictSingleImage
};

Console.WriteLine("Making a prediction");
var predictor = mlContext.Model.CreatePredictionEngine<InputData, Prediction>(model);
var prediction = predictor.Predict(imageData);
Console.WriteLine($"Image {Path.GetFileName(prediction.ImagePath)} recognized as {prediction.PredictedLabelValue} with confidence score of {prediction.Score?.Max()}");

Console.WriteLine("Assessing model accuracy metrics");
var metrics =
    mlContext.MulticlassClassification.Evaluate(predictions,
        labelColumnName: "LabelKey",
        predictedLabelColumnName: "PredictedLabel");


Console.WriteLine($"LogLoss - {metrics.LogLoss}");
Console.WriteLine($"PerClassLogLoss - {string.Join(" , ", metrics.PerClassLogLoss.Select(c => c.ToString()))}");