using Microsoft.ML.Data;

namespace TensorFlowImageClassifier;

public class InputData
{
    [LoadColumn(0)]
    public string ImagePath { get; set; }

    [LoadColumn(1)]
    public string Label { get; set; }
}
