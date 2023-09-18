using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
namespace Zebra.Function
{
    public class TimerTrigger
    {
        private readonly ILogger _logger;

        public TimerTrigger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TimerTrigger>();
        }

        [Function("TimerTrigger")]
        public async Task RunAsync([TimerTrigger("*/10 * * * * *")] MyInfo myTimer)
        {
            
            using CosmosClient client = new(
                accountEndpoint: Environment.GetEnvironmentVariable("CosmosDbConnectionString")!,
                authKeyOrResourceToken: Environment.GetEnvironmentVariable("CosmosKey")!
            );
            // New instance of Database response class referencing the server-side database
            DatabaseResponse response = await client.CreateDatabaseIfNotExistsAsync(
                id: "TimeTrackingEntries"
            );
            // Parse additional response properties
            Database database3 = response.Database;

            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
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
