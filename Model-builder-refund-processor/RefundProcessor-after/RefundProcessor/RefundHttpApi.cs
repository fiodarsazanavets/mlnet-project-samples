using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using RefundProcessor.Models;
using System.Net;
using System.Text.Json;

namespace RefundProcessor;

public static class RefundHttpApi
{
    [Function(nameof(StartRefund))]
    public static async Task<HttpResponseData> StartRefund(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "refunds/start")] HttpRequestData req,
        [DurableClient] DurableTaskClient client)
    {
        var request = await JsonSerializer.DeserializeAsync<RefundRequest>(req.Body,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        if (request is null)
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Invalid payload.");
            return bad;
        }

        var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            new TaskName(nameof(RefundOrchestrations.RefundOrchestrator)),
            request,
            new StartOrchestrationOptions
            {
                InstanceId = request.RefundId
            });

        var res = req.CreateResponse(HttpStatusCode.Accepted);
        await res.WriteStringAsync($"Refund workflow started. InstanceId={instanceId}");
        return res;
    }

    // Approver calls this after reviewing the refund in an approval UI/system.
    [Function(nameof(SubmitDecision))]
    public static async Task<HttpResponseData> SubmitDecision(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "refunds/decision")] HttpRequestData req,
        [DurableClient] DurableTaskClient client)
    {
        var decision = await JsonSerializer.DeserializeAsync<ApprovalDecision>(req.Body,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        if (decision is null || string.IsNullOrWhiteSpace(decision.RefundId))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Invalid payload.");
            return bad;
        }

        // Raise external event to the orchestrator instance
        await client.RaiseEventAsync(
            instanceId: decision.RefundId,
            eventName: "ApprovalDecision",
            eventPayload: decision);

        var ok = req.CreateResponse(HttpStatusCode.OK);
        await ok.WriteStringAsync("Decision submitted.");
        return ok;
    }
}
