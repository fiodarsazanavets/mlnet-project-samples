using Microsoft.ML.TimeSeries;
using Microsoft.ML;

namespace AnomalyDetection;

public static class AnomalyDetector
{
    public static void DetectAnomaliesInData(string inputFilePath)
    {
        MLContext mlContext = new();

        Console.Write("Loading the time-series data.");
        IDataView dataView = mlContext.Data.LoadFromTextFile<InputData>(path: inputFilePath, hasHeader: true, separatorChar: ',');

        int period = DetectIntervalPeriod(mlContext, dataView);

        DetectAnomaly(inputFilePath, mlContext, dataView, period);  
    }

    private static int DetectIntervalPeriod(MLContext mlContext, IDataView data)
    {
        int period = mlContext.AnomalyDetection.DetectSeasonality(data, nameof(InputData.Value));
        Console.WriteLine("Interval period of the series: {0}.", period);
        return period;
    }

    private static void DetectAnomaly(string inputFilePath, MLContext mlContext, IDataView inputData, int period)
    {
        Console.WriteLine("Looking for the anomalies in the data.");
        SrCnnEntireAnomalyDetectorOptions options = new()
        {
            Threshold = 0.3,
            Sensitivity = 64.0,
            DetectMode = SrCnnDetectMode.AnomalyAndMargin,
            Period = period,
        };

        IDataView outputDataView =
            mlContext
                .AnomalyDetection.DetectEntireAnomalyBySrCnn(
                    inputData,
                    nameof(Output.Prediction),
                    nameof(InputData.Value),
                    options);

        IEnumerable<Output> predictions = mlContext.Data.CreateEnumerable<Output>(
            outputDataView, reuseRowObject: false);

        Console.WriteLine("Index,Anomaly,ExpectedValue,UpperBoundary,LowerBoundary");     

        int index = 0;

        using StreamWriter outputFile = new(inputFilePath.Replace(".csv", "-output.csv"));
        List<InputData> inputs = mlContext.Data.CreateEnumerable<InputData>(
            inputData, reuseRowObject: false).ToList();

        foreach (Output p in predictions)
        {
            if (p.Prediction[0] == 1)
            {
                Console.WriteLine("{0},{1},{2},{3},{4}\t<-- anomaly detected!", index,
                    p.Prediction[0], p.Prediction[3], p.Prediction[5], p.Prediction[6]);
            }
            else
            {
                Console.WriteLine("{0},{1},{2},{3},{4}", index,
                    p.Prediction[0], p.Prediction[3], p.Prediction[5], p.Prediction[6]);
                outputFile.WriteLine($"{inputs[index].Timestamp},{inputs[index].Value}");
            }
            ++index;
        }

        Console.WriteLine(string.Empty);
    }
}