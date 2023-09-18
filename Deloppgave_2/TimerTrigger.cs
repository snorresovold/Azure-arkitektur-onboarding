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
            Container container = client.GetContainer("Testainer", "testainer");

            TimeTrackingEntry entry = new TimeTrackingEntry() {
                id = "6",
                Consultant = "John Doe",
                Date = "2023-09-18",
                Account = "Client ABC",
                ActivityCode = "DEV",
                Hours = new Random().Next(1, 10), // Generates a random integer between 1 and 10
                Comment = "Worked on feature implementation"
            };
                TimeTrackingEntry createdItem = await container.CreateItemAsync(
                item: entry,
                partitionKey: new PartitionKey("id")
            );

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
