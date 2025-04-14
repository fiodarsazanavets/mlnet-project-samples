using System;
using System.IO;
using static HouseItemClassifier.ConsoleApp.HouseItemClassifier;

Console.WriteLine("Please specify the path to an image file to classify");
string imagePath = Console.ReadLine();

byte[] imageBytes = await File.ReadAllBytesAsync(imagePath);

ModelInput imageToClassify = new()
{
    ImageSource = imageBytes,
};

Console.WriteLine();
Console.WriteLine("Predicted label:");

var prediction = Predict(imageToClassify);

Console.WriteLine(prediction.PredictedLabel);
Console.WriteLine();

Console.WriteLine("All predicted label scores:");
var sortedScoresWithLabel = PredictAllLabels(imageToClassify);
Console.WriteLine($"{"Class",-40}{"Score",-20}");
Console.WriteLine($"{"-----",-40}{"-----",-20}");

foreach (var score in sortedScoresWithLabel)
{
    Console.WriteLine($"{score.Key,-40}{score.Value,-20}");
}