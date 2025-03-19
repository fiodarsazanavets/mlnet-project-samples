using System;
using System.Linq;
using Microsoft.ML.Data;
using static ObjectDetection.ConsoleApp.ObjectDetection;

Console.WriteLine("Please specify the image to analyze");
string imageFilePath = Console.ReadLine();


var image = MLImage.CreateFromFile(imageFilePath);
ModelInput sampleData = new ()
{
    Image = image,
};

// Make a single prediction on the sample data and print results
var predictionResult = Predict(sampleData);
Console.WriteLine("\n\nPredicted Boxes:\n");
if (predictionResult.PredictedBoundingBoxes == null)
{
    Console.WriteLine("No Predicted Bounding Boxes");
    return;
}

Console.WriteLine($"The most prominent object on the image is {predictionResult.PredictedLabel[0]}");

var boxes =
    predictionResult.PredictedBoundingBoxes.Chunk(4)
       .Select(x => new { XTop = x[0], YTop = x[1], XBottom = x[2], YBottom = x[3] }).Zip(predictionResult.Score, (a, b) => new { Box = a, Score = b });

foreach (var item in boxes)
{
    Console.WriteLine($"XTop: {item.Box.XTop},YTop: {item.Box.YTop},XBottom: {item.Box.XBottom},YBottom: {item.Box.YBottom}, Score: {item.Score}");
}

