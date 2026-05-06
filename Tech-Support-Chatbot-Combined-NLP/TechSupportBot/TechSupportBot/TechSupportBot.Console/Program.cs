using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TechSupportBot.Core.Abstractions;
using TechSupportBot.Core.Data;
using TechSupportBot.Core.Services;

var builder = Host.CreateApplicationBuilder(args);

// Load appsettings.json and environment-specific config
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile(
        $"appsettings.{builder.Environment.EnvironmentName}.json",
        optional: true,
        reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args);

// Register services
builder.Services.AddSingleton<ITicketClassificationService, TicketClassificationService>();
builder.Services.AddSingleton<ISimilarityDetectionService, SimilarityDetectionService>();
builder.Services.AddSingleton<INamedEntityRecognitionService, NamedEntityRecognitionService>();
builder.Services.AddSingleton<IQuestionAnsweringService, QuestionAnsweringService>();

using var host = builder.Build();

var ticketClassificationService =
    host.Services.GetRequiredService<ITicketClassificationService>();

Console.WriteLine("Tech Support Bot");
Console.WriteLine("------------------------------");
Console.WriteLine("Enter a support ticket request:");
Console.Write("> ");

var ticketText = Console.ReadLine();

if (string.IsNullOrWhiteSpace(ticketText))
{
    Console.WriteLine("No ticket text was entered.");
    return;
}

var input = new TicketData
{
    Text = ticketText
};

var prediction = ticketClassificationService.Predict(input);

Console.WriteLine();
Console.WriteLine($"Your ticket was classed as: {prediction.PredictedLabel}");
Console.WriteLine();

Console.WriteLine();
Console.WriteLine("Enter another issue to check if it is a duplicate:");
Console.Write("> ");

var possibleDuplicateText = Console.ReadLine();

if (!string.IsNullOrWhiteSpace(possibleDuplicateText))
{
    var similarityDetectionService =
    host.Services.GetRequiredService<ISimilarityDetectionService>();

    var similarityPrediction = similarityDetectionService.Predict(new SimilarityData
    {
        Sentence1 = ticketText,
        Sentence2 = possibleDuplicateText
    });

    Console.WriteLine();
    Console.WriteLine($"Similarity score: {similarityPrediction.Score:0.000}");

    const float duplicateThreshold = 0.75f;

    if (similarityPrediction.Score >= duplicateThreshold)
    {
        Console.WriteLine("This is likely to be the same issue.");
    }
    else
    {
        Console.WriteLine("This does not appear to be the same issue.");
    }

    Console.WriteLine();
}

var namedEntityRecognitionService =
    host.Services.GetRequiredService<INamedEntityRecognitionService>();

var namedEntityPrediction = namedEntityRecognitionService.Predict(new NamedEntityData
{
    Sentence = ticketText
});

Console.WriteLine($"The issue is related to {
    string.Join(", ", namedEntityPrediction.PredictedLabel.Distinct().Where(s => s != "O"))}");

var questionAnsweringService =
    host.Services.GetRequiredService<IQuestionAnsweringService>();

Console.WriteLine();

Console.WriteLine();
Console.WriteLine("If you have it, please provide supporting information, such as full error message or press ENTER if you don't.");
Console.Write("> ");

var contextText = Console.ReadLine();

if (string.IsNullOrWhiteSpace(contextText))
{
    Console.WriteLine("The next avalable tech support agent will deal with your request.");
    Console.ReadKey();
    return;
}

Console.WriteLine();
Console.WriteLine("Tell me if you have any questions about this or press ENTER if you don't.");
Console.Write("> ");

var questionText = Console.ReadLine();

if (string.IsNullOrWhiteSpace(questionText))
{
    Console.WriteLine("Very well. The information has been passed to the tech support.");
    Console.ReadKey();
    return;
}

var answer = questionAnsweringService.Predict(new QuestionAnsweringData
{
    Question = questionText,
    Context = ticketText
});

Console.WriteLine(answer.Answer.GetValues()[0].ToString());

Console.ReadKey();