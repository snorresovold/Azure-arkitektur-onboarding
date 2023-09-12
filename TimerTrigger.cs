using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using IronXL;
using static TimeTrackingEntry;
using static RequestObject;
using System.Net.Http.Json;
using System.Text.Json;

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
            //Create blob client for our file
            BlobContainerClient blobContainer = new BlobContainerClient(connectionString, containerName);
            BlobClient blobClient = blobContainer.GetBlobClient(fileName);
            MemoryStream memoryStream = new MemoryStream();
            if (await blobClient.ExistsAsync())
            {

                //Download file from blob and parse to XLWorkbook
                blobClient.DownloadTo("/tmp/test.xlsx"); ;
                _logger.LogInformation("Downloaded blob: " + fileName + " from azure blob container.");
                // convert stream to string

                //Todo: Create object TimeTrackingEntry we can read the excel data into
                //Todo: Read through each row in the excel sheet and create list of TimeTrackingEntry to send to POG
                //Todo: Close connection to blob
            }
            WorkBook wb = WorkBook.Load("/tmp/test.xlsx");//Excel file path
            WorkSheet ws = wb.WorkSheets.First(); //by sheet name
            // start is 4, loop trough until you dont get any data and increase every loop
            // [A[start]:I[start]]
            List<TimeTrackingEntry> TimeTrackingEntries = new List <TimeTrackingEntry>();
            for (int i = 4; i < 9; i++) 
            {
                List<string> temp = new List<string>();
                foreach (var cell in ws[$"A{i}:I{i}"])
                {
                    temp.Add(cell.Text);
                    _logger.LogInformation("value is: {0}", cell.Text);

                    _logger.LogInformation($"{i}");
                }
                TimeTrackingEntry myObj = new TimeTrackingEntry(temp[0], temp[1], temp[2], temp[3], temp[4], temp[5]) {};
                TimeTrackingEntries.Add(myObj);
            }
            foreach (var entry in TimeTrackingEntries) { // send request to POG with the entry
                _logger.LogInformation(entry.Account.ToString());
            }
            //_logger.LogInformation(TimeTrackingEntries.Count().ToString());
            List<RequestObject> requestObjects = new List <RequestObject>();
            foreach(var entry in TimeTrackingEntries) {
                RequestObject timeEntry = new RequestObject
                {
                    Id = 0,
                    ActivityCode = entry.ActivityCode,
                    ProjectCode = "2",
                    EmployeeCode = 9,
                    CustomerCode = 10000,
                    HourType = "Overtid 100%",
                    Date = DateTime.Parse("2023-02-27"),
                    IsLocked = false,
                    Comment = entry.Comment,
                    InternalComment = "",
                    LastChanged = DateTime.Parse(entry.Date),
                    ExcludedFromPayroll = false,
                    IsTransferedToPayroll = false,
                    IsInvoiced = false,
                    HourlyRate = 1250.0000m,
                    HourlyCost = 0.0000m,
                    Minutes = 9999999,
                    BillableHours = 7.0000m,
                    BillableAmount = 8750.0000m,
                    BreakTime = 0
                };
                requestObjects.Add(timeEntry);
            }

            string pogEndnpoint = "https://api-demo.poweroffice.net/";
            string clientId = "3c04c56d-90b6-43a9-8c4a-d61cfb593f5c";
            string clientSecret = "67705ed4-5753-4294-a64e-ec70647427e0";
            byte[] basicAuth = System.Text.Encoding.UTF8.GetBytes(clientId + ":" + clientSecret);
            string basicAuthEncoded = System.Convert.ToBase64String(basicAuth);
            //Create http token client and set basic auth header
            var httpTokenClient = new HttpClient()
            {
                BaseAddress = new Uri(pogEndnpoint)
            };
            httpTokenClient.DefaultRequestHeaders.Add("Authorization", "Basic " + basicAuthEncoded);
            //POG API doc specifies: "Set the content type header to be 'application/x-www-form-urlencoded' and body must contain 'grant_type=client_credentials'"
            HttpContent data = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"grant_type", "client_credentials"}
            });

            //Create client for get and post requests towards POG
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(pogEndnpoint)
            };
            foreach (var item in requestObjects)
            {
                HttpContent jsonContent = JsonContent.Create(item);
                HttpResponseMessage postResponse = httpClient.PostAsync(pogEndnpoint + "/TimeTracking/TimeTrackingEntry/",jsonContent).Result;
                string jsonResponse = await postResponse.Content.ReadAsStringAsync();
                _logger.LogInformation(jsonResponse);
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
}