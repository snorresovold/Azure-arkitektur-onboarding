using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public class TimerTrigger
    {
        private readonly ILogger _logger;
        private readonly ExcelParser _excelParser;
        private readonly POGApiClient _pogApiClient;

        public TimerTrigger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TimerTrigger>();
            _excelParser = new ExcelParser(_logger);
            _pogApiClient = new POGApiClient(_logger);
        }

        [Function("TimerTrigger")]
        public async Task RunAsync([TimerTrigger("*/10 * * * * *")] MyInfo myTimer)
        {

            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            string connectionString = Environment.GetEnvironmentVariable("connectionString");
            string containerName = Environment.GetEnvironmentVariable("containerName");
            string fileName = Environment.GetEnvironmentVariable("fileName");

            List<TimeTrackingEntry> TimeTrackingEntries = _excelParser.ParseExcel(connectionString, containerName, fileName);

            List<RequestObject> requestObjects = _excelParser.CreateRequestObjects(TimeTrackingEntries);

            string pogEndnpoint = Environment.GetEnvironmentVariable("pogEndpoint");
            string clientId = Environment.GetEnvironmentVariable("clientId");
            string clientSecret = Environment.GetEnvironmentVariable("clientSecret");


            string accessToken = await _pogApiClient.GetAccessTokenAsync(pogEndnpoint,
                                                                         clientId,
                                                                         clientSecret);

            await _pogApiClient.PostTimeTrackingEntriesAsync(requestObjects, accessToken);
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
}