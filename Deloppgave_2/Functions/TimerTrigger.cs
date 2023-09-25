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
            List<string> idList = new List<string> { "1", "2", "3", "4", "5" };
            List<TimeTrackingEntry> result = await CosmosHandler.ReadMultipleEntries<TimeTrackingEntry>(container, idList, "Sindre Langeveld");
           List<RequestObject> requestObjects = result.Select(entry => new RequestObject
            {
                Id = int.Parse(entry.id),
                ActivityCode = entry.ActivityCode,
                ProjectCode = "2",
                EmployeeCode = 9,
                CustomerCode = 10000,
                HourType = "",
                Date = DateTime.Parse("2023-02-27"),
                IsLocked = false,
                Comment = entry.Comment,
                InternalComment = "",
                LastChanged = DateTime.Parse("2023-03-03T09:35:03 +00:00"),
                ExcludedFromPayroll = false,
                IsTransferedToPayroll = false,
                IsInvoiced = false,
                HourlyRate = 1250.0000m,
                HourlyCost = 0.0000m,
                Minutes = 9999999,
                BillableHours = 7.0000m,
                BillableAmount = 8750.0000m,
                BreakTime =  0
            }).ToList(); 
            
            string pogEndnpoint = Environment.GetEnvironmentVariable("pogEndpoint");
            string clientId = Environment.GetEnvironmentVariable("clientId");
            string clientSecret = Environment.GetEnvironmentVariable("clientSecret");
            string accessToken = await _pogApiClient.GetAccessTokenAsync(pogEndnpoint,
                                                                         clientId,
                                                                         clientSecret);
            await _pogApiClient.PostTimeTrackingEntriesAsync(requestObjects, accessToken);
            _logger.LogInformation("Sent: " + requestObjects + "CosmosDB.");
        }
        static string ConvertDateFormat(string inputDate)
    {
        // Parse the input date string
        DateTime originalDate = DateTime.ParseExact(inputDate, "M/d/yyyy h:mm:ss tt", null);

        // Convert it to the desired format
        string convertedDate = originalDate.ToString("yyyy-MM-dd");

        return convertedDate;
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