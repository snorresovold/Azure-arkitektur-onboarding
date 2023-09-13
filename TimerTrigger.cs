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

            string connectionString = "DefaultEndpointsProtocol=https;AccountName=storageaccountsnorre;AccountKey=qdUxk/A+8WI9S0+XTSzxlH7tE3OYFkWKFIDW0ia0NUSCQn3Us1Q/trOOfxk67l8ZRIjaeHwaNZk5+ASt2kzhhA==;EndpointSuffix=core.windows.net";
            string containerName = "containtersnorre";
            string fileName = "exampleTransferSheet.xlsx";

            string TimeTrackingEntries = _excelParser.ParseExcel(connectionString, containerName, fileName);

            // List<RequestObject> requestObjects = _excelParser.CreateRequestObjects(TimeTrackingEntries);

            string pogEndnpoint = "https://api-demo.poweroffice.net/";
            string clientId = "3c04c56d-90b6-43a9-8c4a-d61cfb593f5c";
            string clientSecret = "67705ed4-5753-4294-a64e-ec70647427e0";

            string accessToken = await _pogApiClient.GetAccessTokenAsync(pogEndnpoint,
                                                                         clientId,
                                                                         clientSecret);

        //    await _pogApiClient.PostTimeTrackingEntriesAsync(requestObjects, accessToken);
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