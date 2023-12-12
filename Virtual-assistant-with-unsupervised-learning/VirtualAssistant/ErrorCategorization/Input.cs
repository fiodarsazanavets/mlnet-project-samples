using Microsoft.ML.Data;

namespace ErrorCategorization;

public class Input
{
    [LoadColumn(0)]
    public string ErrorCode { get; set; } = string.Empty;

    [LoadColumn(1)]
    public string ErrorMessage { get; set; } = string.Empty;
}
