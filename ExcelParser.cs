using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;

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

    public List<TimeTrackingEntry> ParseExcel(string connectionString, string containerName, string fileName)
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
        List<TimeTrackingEntry> TimeTrackingEntries = new List<TimeTrackingEntry>();

        int numRows = ws.LastRowUsed().RowNumber();
        int currentRow = 0;
        List<List<string>> result = new List<List<string>>();
        while (currentRow < numRows)
        {
            List<string> currentRowData = new List<string>();
            string letters = ws.LastColumnUsed().ColumnLetter();
            int numCols = ExcelColumnNameToNumber(letters);
            int currentCol = 0;

            while (currentCol < numCols)
            {
                string value = ws.Cell(currentRow, currentCol).GetValue<string>();
                _logger.LogInformation("fortnite" + value);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    currentRowData.Add(value);
                }
                foreach(var x in currentRowData) {
                    _logger.LogInformation(x);
                }
                currentCol++;
            }

            if (currentRowData.Count > 0)
            {
                result.Add(currentRowData);
            }

            currentRow++;
        }
        return TimeTrackingEntries;
        // int columnNumber = 1;
        // int rowNumber = 1;
        // for (int i = 0; i < 5; i++) 
        // {
        //     List<string> rowData = new List<string>();
        //     while (true)
        //     {
        //         var cell = row.Cell(columnNumber);
        //         string value = cell.GetValue<string>();

        //         if (string.IsNullOrWhiteSpace(value))
        //         {
        //             break;
        //         }

        //         rowData.Add(value);
        //         _logger.LogInformation(value, columnNumber);
        //     }
        //     TimeTrackingEntry entry = new TimeTrackingEntry(rowData[0], rowData[1], rowData[2], rowData[3], rowData[4], rowData[5]) {};
        //     // _logger.LogInformation(rowData[0], rowData[1], rowData[2], rowData[3], rowData[4], rowData[5]);
        //     TimeTrackingEntries.Add(entry);
        // }
    }
}
    public static int ExcelColumnNameToNumber(string columnName)
    {
        if (string.IsNullOrEmpty(columnName)) throw new ArgumentNullException("columnName");

        columnName = columnName.ToUpperInvariant();

        int sum = 0;

        for (int i = 0; i < columnName.Length; i++)
        {
            sum *= 26;
            sum += (columnName[i] - 'A' + 1);
        }

        return sum;
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
