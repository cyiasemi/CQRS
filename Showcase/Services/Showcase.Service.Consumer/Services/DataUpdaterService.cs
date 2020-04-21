using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Showcase.Service.Application.Services
{
    public class DataUpdaterService : IDataUpdaterService
    {

        private readonly TimeSpan _updateInterval;
        private readonly string _updateUrl;
        private readonly ILogger<DataUpdaterService> _logger;
        private DateTimeOffset _lastUpdate;
        private DateTimeOffset _nextUpdate;

        public DataUpdaterService(DateTimeOffset lastUpdate, TimeSpan updateInterval, string updateUrl, ILogger<DataUpdaterService> logger)
        {
            _lastUpdate = lastUpdate;
            _updateInterval = updateInterval;
            _updateUrl = updateUrl;
            _logger = logger;
            _nextUpdate = lastUpdate.Add(updateInterval);
            _logger.LogInformation($"Data updater service started. Next update: {_nextUpdate} , Last update: {_lastUpdate}");
        }

        public void StartMonitoring()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    if (_nextUpdate <= DateTimeOffset.UtcNow)
                    {
                        await Update();
                    }
                    await Task.Delay(TimeSpan.FromSeconds(10));
                }

            });
        }
        public async Task<(DateTimeOffset LastUpdate, DateTimeOffset NextUpdate)> Update()
        {
            try
            {
                using HttpClient client = new HttpClient();
                await client.PostAsync(_updateUrl, null);
                _logger.LogInformation($"UPDATE DATA - Data updater service. Next update: {_nextUpdate} , Last update: {_lastUpdate}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Sending update command failed: Error: {ex.Message}, InnerException {ex.InnerException?.Message}, Stack {ex.StackTrace}");
            }
            finally
            {
                _lastUpdate = DateTimeOffset.UtcNow;
                _nextUpdate = DateTimeOffset.UtcNow.Add(_updateInterval);
            }
            return (LastUpdate: _lastUpdate, NextUpdate: _nextUpdate);
        }
    }
}
