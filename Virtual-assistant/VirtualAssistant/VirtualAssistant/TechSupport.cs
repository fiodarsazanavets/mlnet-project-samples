namespace VirtualAssistant;

using TechSupportIssues.ConsoleApp;

internal static class TechSupport
{
    public static void ClassifyProblem()
    {
        Console.WriteLine("Please provide the error title");
        var title = Console.ReadLine();
        Console.WriteLine("Please provide the error description");
        var description = Console.ReadLine();

        TechSupportIssues.ModelInput input = new TechSupportIssues.ModelInput()
        {
            Title = title,
            Description = description,
        };

        var prediction = TechSupportIssues.Predict(input);

        Console.WriteLine($"The problem falls under the {prediction.PredictedLabel} category");
    }
}