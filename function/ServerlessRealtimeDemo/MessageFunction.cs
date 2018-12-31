using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace ServerlessRealtimeDemo
{
    public static class MessageFunction
    {
        [FunctionName("message")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequest req, 
                                                    [SignalR(HubName = "broadcast")]IAsyncCollector<SignalRMessage> signalRMessages, 
                                                    ILogger log)
        {
            string requestBody = new StreamReader(req.Body).ReadToEnd();

            if (string.IsNullOrEmpty(requestBody))
            {
                return new BadRequestObjectResult("Please pass a payload to broadcast in the request body.");
            }

            log.LogInformation($"Message with payload {requestBody}");

            await signalRMessages.AddAsync(new SignalRMessage()
            {
                Target = "notify",
                Arguments = new object[] { requestBody }
            });

            return new OkResult();
        }
    }
}
