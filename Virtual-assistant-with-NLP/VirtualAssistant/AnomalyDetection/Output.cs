using Microsoft.ML.Data;

namespace AnomalyDetection;

public class Output
{
    [VectorType(7)]
    public double[] Prediction { get; set; } = [];
}
