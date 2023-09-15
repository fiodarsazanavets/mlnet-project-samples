using Microsoft.ML.Data;

namespace Ranking;

internal class Input
{
    [LoadColumn(0), ColumnName("Label")]
    public float Score { get; set; }

    [LoadColumn(1), ColumnName("GroupId")]
    public string GroupId { get; set; }

    [LoadColumn(2, 133)]
    [VectorType(133)]
    public float[] Features { get; set; }
}
