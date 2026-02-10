namespace RefundProcessor.Models;

public record ApprovalDecision(
    string RefundId,
    bool Approved,
    string? Comment,
    string ApprovedBy,
    DateTimeOffset DecidedAtUtc
);
