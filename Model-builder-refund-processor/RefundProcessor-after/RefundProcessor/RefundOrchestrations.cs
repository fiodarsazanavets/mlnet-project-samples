using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using RefundProcessor.Models;

namespace RefundProcessor;

internal class RefundOrchestrations
{
    private const string ApprovalEventName = "ApprovalDecision";

    [Function(nameof(RefundOrchestrator))]
    public static async Task<RefundResult> RefundOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var request = context.GetInput<RefundRequest>()
            ?? throw new InvalidOperationException("RefundRequest input is required.");

        var requiresApproval = await context.CallActivityAsync<bool>(
            nameof(PredictRequiresApprovalActivity),
            request);

        // 1) If NOT requiring approval => auto-process immediately
        if (!requiresApproval)
        {
            await context.CallActivityAsync(
                nameof(ProcessRefundActivity),
                (request, decision: (ApprovalDecision?)null));

            return new RefundResult(request.RefundId, RefundStatus.AutoProcessed, "Auto-approved by model.");
        }

        // 1. Always create an approval request
        var approval = await context.CallActivityAsync<ApprovalRequest>(
            nameof(CreateApprovalRequestActivity), request);

        // 2. Wait for approval decision (external event), with a timeout
        var timeoutAt = context.CurrentUtcDateTime.AddHours(24);
        using var cts = new CancellationTokenSource();

        var decisionTask = context.WaitForExternalEvent<ApprovalDecision>(ApprovalEventName);
        var timeoutTask = context.CreateTimer(timeoutAt, cts.Token);

        var completed = await Task.WhenAny(decisionTask, timeoutTask);

        if (completed == timeoutTask)
        {
            return new RefundResult(request.RefundId, RefundStatus.TimedOut, "No approval decision received within 24 hours.");
        }

        cts.Cancel();

        var decision = await decisionTask;

        if (!decision.Approved)
        {
            await context.CallActivityAsync(
                nameof(RecordRefundRejectedActivity),
                (request, decision));

            return new RefundResult(request.RefundId, RefundStatus.Rejected, decision.Comment);
        }

        // 3. Process the refund only if approved
        await context.CallActivityAsync(
            nameof(ProcessRefundActivity),
            (request, decision));

        return new RefundResult(request.RefundId, RefundStatus.ApprovedAndProcessed, decision.Comment);
    }

    [Function(nameof(CreateApprovalRequestActivity))]
    public static ApprovalRequest CreateApprovalRequestActivity(
        [ActivityTrigger] RefundRequest request,
        FunctionContext executionContext)
    {
        // In real life: insert into DB / send to queue / create ticket in system, etc.
        return new ApprovalRequest(
            RefundId: request.RefundId,
            ApproverQueue: "refund-approvals",
            Summary: $"Refund {request.RefundId} for Order {request.OrderId}, Amount {request.Amount} {request.Currency}, Reason={request.ReasonCode}, Category={request.Category}",
            CreatedAtUtc: DateTimeOffset.UtcNow
        );
    }

    [Function(nameof(ProcessRefundActivity))]
    public static Task ProcessRefundActivity(
        [ActivityTrigger] (RefundRequest request, ApprovalDecision decision) input,
        FunctionContext executionContext)
    {
        // In real life: call payment provider, update ledger, emit event, etc.
        // This is a stub to keep the example focused.
        return Task.CompletedTask;
    }

    [Function(nameof(RecordRefundRejectedActivity))]
    public static Task RecordRefundRejectedActivity(
        [ActivityTrigger] (RefundRequest request, ApprovalDecision decision) input,
        FunctionContext executionContext)
    {
        // In real life: update status in DB, notify customer, emit event, etc.
        return Task.CompletedTask;
    }

    [Function(nameof(PredictRequiresApprovalActivity))]
    public static bool PredictRequiresApprovalActivity(
        [ActivityTrigger] RefundRequest request,
        FunctionContext context)
    {
        var input = new RefundDecisionModel.ModelInput
        {
            Price = (float)request.Amount,
            Category = request.Category,
            DaysSincePurchase = request.DaysSincePurchase,
            PriorRefundCount = request.PriorRefundCount,
            CustomerTenureDays = request.CustomerTenureDays,
            ReasonCode = request.ReasonCode,
            IsInternationalOrder = request.IsInternationalOrder,
            ShippingCountryMismatch = request.ShippingCountryMismatch,
            IsHighRiskPayment = request.IsHighRiskPayment,

            // Label is not used for inference. Leave default.
            RequiresApproval = false
        };

        var prediction = RefundDecisionModel.Predict(input);

        // In Model Builder binary classification, PredictedLabel is the decision.
        // If PredictedLabel == true => "RequiresApproval" predicted.
        return prediction.PredictedLabel;
    }
}
