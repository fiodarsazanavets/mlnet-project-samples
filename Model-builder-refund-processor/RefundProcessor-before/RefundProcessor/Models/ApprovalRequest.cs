namespace RefundProcessor.Models;

public record ApprovalRequest(
    string RefundId,
    string ApproverQueue,
    string Summary,
    DateTimeOffset CreatedAtUtc
);
