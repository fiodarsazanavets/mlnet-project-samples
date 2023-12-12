using Microsoft.ML.Data;

namespace ErrorCategorization;

public class Output
{
    [ColumnName("PredictedLabel")]
    public uint PredictedClusterId { get; set; } = 0;

    [ColumnName("Score")]
    public float[] Distances = [];
}
