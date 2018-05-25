using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessRealtimeDemo.UWP
{
    public class SignalRService
    {
        private const string _baseUrl = "http://localhost:7071/api/";
        private readonly HttpClient _httpClient = new HttpClient();
        private HubConnection _hubConnection;

        public async Task Init()
        {
            var info = await GetConnectionInfo();
            _hubConnection = new HubConnectionBuilder().WithUrl(info.Endpoint, (options) => options.AccessTokenProvider = () => Task.FromResult<string>(info.AccessKey))
                                                       .Build();

            await _hubConnection.StartAsync();

            _hubConnection.On("notify", (string data) => MessageReceived?.Invoke(this, data));
        }

        private async Task<SignalRConnectionInfo> GetConnectionInfo()
        {
            var requestUrl = $"{_baseUrl}negotiate";
            var response = await _httpClient.GetStringAsync(requestUrl);
            return JsonConvert.DeserializeObject<SignalRConnectionInfo>(response);
        }

        public event EventHandler<string> MessageReceived;

        public async Task Send(string message)
        {
            var requestUrl = $"{_baseUrl}message";
            var requestContent = new StringContent(message);
            await _httpClient.PostAsync(requestUrl, requestContent);
        }
    }
}
