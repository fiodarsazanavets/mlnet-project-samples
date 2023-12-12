using ErrorCategorization;

namespace VirtualAssistant;

internal static class ErrorCategorizer
{
    public static void CategorizeErrors()
    {
        Console.WriteLine("Please specify the path to the training data.");
        string inputFilePath = Console.ReadLine() ?? string.Empty;

        ErrorPatternDetector.DetectPatterns(inputFilePath);

        Console.WriteLine("What error code do you want to check?");
        string errorCode = Console.ReadLine() ?? string.Empty;
        Console.WriteLine("What error message do you want to check?");
        string errorMessage = Console.ReadLine() ?? string.Empty;

        Input input = new()
        {
            ErrorCode = errorCode,
            ErrorMessage = errorMessage
        };

        Output prediction = ErrorPatternMatcher.PredictErrorCategory(input);

        Console.WriteLine($"Predicted cluster id is {prediction.PredictedClusterId}");
        Console.WriteLine("Distances from different clusters are:");

        foreach (float distance in prediction.Distances)
        {
            Console.WriteLine(distance);
        }

        Console.WriteLine("This cluster contains the following items:");

        IList<Input> items = ClusterData.GetItemsInCluster(prediction.PredictedClusterId);

        foreach (Input item in items)
        {
            Console.WriteLine($"Error Code: {item.ErrorCode}; Error Message: {item.ErrorMessage}");
        }
    }
}
