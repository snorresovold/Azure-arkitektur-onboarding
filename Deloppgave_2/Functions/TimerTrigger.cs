using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using System;
using static CosmosHandler;

namespace Zebra.Function
{
    public class TimerTrigger
    {
        private readonly ILogger _logger;
        private readonly POGApiClient _pogApiClient;

        public TimerTrigger(ILoggerFactory loggerFactory)
        
        {
            _logger = loggerFactory.CreateLogger<TimerTrigger>();
            _pogApiClient = new POGApiClient(_logger);
        }

        [Function("TimerTrigger")]
        public async Task RunAsync([TimerTrigger("*/10 * * * * *")] MyInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            (Container container, Database db) = await CosmosHandler.Init();
            List<string> idList = new List<string> { "1", "2", "3", "4" };
            List<TimeTrackingEntry> result = await CosmosHandler.ReadMultipleEntries<TimeTrackingEntry>(container, idList, "Sindre Langeveld");
            
            string pogEndnpoint = Environment.GetEnvironmentVariable("pogEndpoint");
            string clientId = Environment.GetEnvironmentVariable("clientId");
            string clientSecret = Environment.GetEnvironmentVariable("clientSecret");


            string accessToken = await _pogApiClient.GetAccessTokenAsync(pogEndnpoint,
                                                                         clientId,
                                                                         clientSecret);

            await _pogApiClient.PostTimeTrackingEntriesAsync(requestObjects, accessToken);
        }
    }

    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }
        public bool IsPastDue { get; set; }
    }
    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }
        public DateTime Next { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}