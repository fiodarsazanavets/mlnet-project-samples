namespace VirtualAssistant;

using TechSupportIssues.ConsoleApp;

internal static class TechSupport
{
    public static void ClassifyProblem()
    {
        Console.WriteLine("Please provide the error title");
        string? title = Console.ReadLine();
        Console.WriteLine("Please provide the error description");
        string? description = Console.ReadLine();

        TechSupportIssues.ModelInput input = new TechSupportIssues.ModelInput()
        {
            Title = title,
            Description = description,
        };

        TechSupportIssues.ModelOutput prediction = TechSupportIssues.Predict(input);

        Console.WriteLine($"The problem falls under the {prediction.PredictedLabel} category");
    }
}