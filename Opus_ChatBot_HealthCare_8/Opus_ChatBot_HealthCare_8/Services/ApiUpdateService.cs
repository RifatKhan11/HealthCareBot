using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Opus_ChatBot_HealthCare_8.Services
{
    public class ApiUpdateService : IHostedService, IDisposable
    {
        private readonly ILogger<ApiUpdateService> _logger;
        private Timer _timer;
        private readonly HttpClient _httpClient;

        public ApiUpdateService(ILogger<ApiUpdateService> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ApiUpdateService is starting.");

            // Schedule the task to run every 6 hours (21600 seconds)
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(12));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            _logger.LogInformation("ApiUpdateService is running at: {time}", DateTimeOffset.Now);

            try
            {
                var response = await _httpClient.GetAsync("https://demotota.opuserp.com/api/UpdateApiData");
                response.EnsureSuccessStatusCode();
                _logger.LogInformation("API call succeeded.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while calling the API.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ApiUpdateService is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _httpClient?.Dispose();
        }
    }
}
