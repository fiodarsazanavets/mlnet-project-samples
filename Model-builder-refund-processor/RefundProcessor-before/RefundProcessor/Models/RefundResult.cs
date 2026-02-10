namespace RefundProcessor.Models;

public record RefundResult(
    string RefundId,
    RefundStatus Status,
    string? Comment
);

public enum RefundStatus
{
    ApprovedAndProcessed,
    Rejected,
    TimedOut
}