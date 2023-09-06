using System;
using System.IO; // Added for Stream and File classes
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Company.Function
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
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            string connectionString = "DefaultEndpointsProtocol=https;AccountName=storageaccountsnorre;AccountKey=qdUxk/A+8WI9S0+XTSzxlH7tE3OYFkWKFIDW0ia0NUSCQn3Us1Q/trOOfxk67l8ZRIjaeHwaNZk5+ASt2kzhhA==;EndpointSuffix=core.windows.net";
            string containerName = "containtersnorre";
            string fileName = "exampleTransferSheet.xlsx";
            BlobContainerClient blobContainer = new(connectionString, containerName);
            BlobClient blobClient = blobContainer.GetBlobClient(fileName);
            //await blobClient.DownloadAsync(@"C:/Desktop");
           MemoryStream memoryStream = new MemoryStream();
            if (await blobClient.ExistsAsync()){
                
                //Download file from blob and parse to XLWorkbook
                await blobClient.DownloadToAsync(memoryStream);
                _logger.LogInformation("Downloaded blob: " + fileName + " from azure blob container.");
                using var excelWbook = new XLWorkbook(memoryStream);
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
