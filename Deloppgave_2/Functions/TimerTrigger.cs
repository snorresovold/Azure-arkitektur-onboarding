using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;

namespace Zebra.Function
{
    public class TimerTrigger
    {
        private readonly ILogger _logger;
        private CosmosHandler _cosmosHandler;
        public TimerTrigger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TimerTrigger>();
            _cosmosHandler = new CosmosHandler();
        }
        [Function("TimerTrigger")]
        public async Task RunAsync([TimerTrigger("*/10 * * * * *")] MyInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            (Container container, Database database) = await _cosmosHandler.Init();
            dynamic placeholder = _cosmosHandler.GenerateRandomProduct();
            _cosmosHandler.CreateTimeTrackingEntry(placeholder, container);
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