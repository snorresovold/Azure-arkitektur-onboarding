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

            // TODO get this into the cosmoshandler
            Product item = new(
                id: "1",
                category: "gear-surf-surfboards",
                name: "Sunnox Surfboard",
                quantity: 8,
                sale: true
            );



            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
    public class MyInfoa
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
