using Microsoft.ML.Data;

namespace Ranking;

internal class Output
{
    [ColumnName("Score")]
    public float Score { get; set; }
}
