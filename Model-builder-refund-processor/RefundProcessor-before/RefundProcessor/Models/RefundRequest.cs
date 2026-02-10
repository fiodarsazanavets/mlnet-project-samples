namespace RefundProcessor.Models;

public record RefundRequest(
    string RefundId,
    string OrderId,
    decimal Amount,
    string Currency,
    string CustomerId,
    string ReasonCode,
    string Category
);