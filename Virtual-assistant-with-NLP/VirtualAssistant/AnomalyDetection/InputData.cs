using Microsoft.ML.Data;

namespace AnomalyDetection;

public class InputData
{
    [LoadColumn(0)]
    public string Timestamp;

    [LoadColumn(1)]
    public double Value;
}