using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;

namespace ServerlessRealtimeDemo
{
    public static class NegotiateFunction
    {
        [FunctionName("negotiate")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")]HttpRequest req, 
                                        [SignalRConnectionInfo(HubName = "broadcast")]SignalRConnectionInfo info, 
                                        ILogger log)
        {
            log.LogInformation($"Negotiate info for endpoint: {info.Url}");

            return info != null
                ? (ActionResult)new OkObjectResult(info)
                : new NotFoundObjectResult("Failed to load SignalR Info.");
        }
    }
}
