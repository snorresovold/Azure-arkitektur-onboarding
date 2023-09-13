using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;

public class ExcelParser
{
    private readonly ILogger _logger;

    public ExcelParser(ILogger logger)
    {
        _logger = logger;
    }

    // public List<TimeTrackingEntry> ParseExcel(string connectionString, string containerName, string fileName)
    // // {
    //     string blobPath = "/tmp/test.xlsx"; // Define where to download the blob file locally

    //     BlobContainerClient blobContainer = new BlobContainerClient(connectionString, containerName);
    //     BlobClient blobClient = blobContainer.GetBlobClient(fileName);

    //     if (blobClient.Exists())
    //     {
    //         blobClient.DownloadTo(blobPath);
    //         _logger.LogInformation("Downloaded blob: " + fileName + " from azure blob container.");
    //     }

    //     // Load the workbook from the blobPath
    //     XLWorkbook wb = new XLWorkbook(blobPath);

    //     // Get the first worksheet
    //     IXLWorksheet ws = wb.Worksheet(1);

    //     List<TimeTrackingEntry> TimeTrackingEntries = new List<TimeTrackingEntry>();
    //     int startColumn = 4;
    //     int endColumn = 9;

    //     for (int i = startColumn; i <= endColumn; i++) // Iterate through columns
    //     {
    //         for (int j = 4; ; j++) // Infinite loop to read rows until there is no data
    //         {
    //             List<string> temp = new List<string>();
    //             bool hasData = true; // Initialize to true

    //             IXLCell cell = ws.Cell(j, i);

    //             // List<string> temp = new List<string>();
    //             // bool hasData = true;

    //             temp.Add(cell.Value.ToString());
    //             _logger.LogInformation("value is: {i}", cell.Value);

    //             if (string.IsNullOrWhiteSpace(cell.Value.ToString()))
    //             {
    //                 hasData = false;
    //             }

    //             if (!hasData)
    //             {
    //                 break; // Exit loop if no data is found in the row
    //             }

    //             TimeTrackingEntry myObj = new TimeTrackingEntry(temp[0], temp[1], temp[2], temp[3], temp[4], temp[5]) {};
    //             TimeTrackingEntries.Add(myObj);
    //         }
    //     }
    //     return TimeTrackingEntries;
    // }

    public List<string> ParseExcel(string connectionString, string containerName, string fileName)
    {
        {
        string blobPath = "/tmp/test.xlsx"; // Define where to download the blob file locally

        BlobContainerClient blobContainer = new BlobContainerClient(connectionString, containerName);
        BlobClient blobClient = blobContainer.GetBlobClient(fileName);

        if (blobClient.Exists())
        {
            blobClient.DownloadTo(blobPath);
            _logger.LogInformation("Downloaded blob: " + fileName + " from azure blob container.");
        }

        // Load the workbook from the blobPath
        XLWorkbook wb = new XLWorkbook(blobPath);

        // Get the first worksheet
        IXLWorksheet ws = wb.Worksheet(1);
        IXLRow row = ws.Row(4);
        List<string> rowData = new List<string>();
        int columnNumber = 1;

        while (true)
        {
            var cell = row.Cell(columnNumber);
            string value = cell.GetValue<string>();

            if (string.IsNullOrWhiteSpace(value))
            {
                break;
            }

            rowData.Add(value);
            columnNumber++;
            _logger.LogInformation(value, columnNumber);
        }
       return rowData;
    }
}

    public static List<RequestObject> CreateRequestObjects(List<TimeTrackingEntry> TimeTrackingEntries)
    {
        List<RequestObject> requestObjects = new List<RequestObject>();

        foreach (var entry in TimeTrackingEntries)
        {
            RequestObject timeEntry = new RequestObject
            {
                Id = 0,
                ActivityCode = "100",
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

        return requestObjects;
    }
}
