namespace VirtualAssistant;

public static class AnomalyDetector
{
    public static void DetectAnomalies()
    {
        Console.WriteLine("Please specify the path to the data.");
        string inputFilePath = Console.ReadLine() ?? string.Empty;

        AnomalyDetection.AnomalyDetector.DetectAnomaliesInData(inputFilePath);
    }
}