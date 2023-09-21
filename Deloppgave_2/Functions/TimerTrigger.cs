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
        private static CosmosHandler _cosmosHandler;

        public TimerTrigger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TimerTrigger>();
            if (_cosmosHandler == null)
            {
                _cosmosHandler = new CosmosHandler();
            }
        }

        [Function("TimerTrigger")]
        public async Task RunAsync([TimerTrigger("*/10 * * * * *")] MyInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            (Container container, Database db) = await CosmosHandler.Init();
            // dynamic item = CosmosHandler.GenerateRandomProduct();
            // await CosmosHandler.CreateTimeTrackingEntry(item, container);
            TimeTrackingEntry data = await CosmosHandler.ReadTimeTrackingEntry<TimeTrackingEntry>(container, "1", "Sindre Langeveld");
            string[] ids = { "1", "2", "3", "4" };
            List<TimeTrackingEntry> result = await CosmosHandler.ReadMultipleEntries<TimeTrackingEntry>(container, ids, "Sindre Langeveld");

            _logger.LogInformation(data.ActivityCode);
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