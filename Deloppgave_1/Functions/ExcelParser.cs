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
            int currentRow = 2;
            List<List<string>> result = new List<List<string>>();

            while (currentRow < numRows) // Changed the condition to include the last row
            {
                List<string> currentRowData = new List<string>();
                string letters = ws.LastColumnUsed().ColumnLetter();
                int numCols = ExcelColumnNameToNumber(letters);
                int currentCol = 1;

                while (currentCol < numCols) // Changed the condition to include the last column
                {
                    string value = ws.Cell(currentRow, currentCol).GetString();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        if (!currentRowData.Contains(value))
                            currentRowData.Add(value);
                    }

                    currentCol++;
                }

                if (currentRowData.Count > 0)
                {
                    result.Add(currentRowData);
                }

                currentRow++;
            }
            // Moved logging outside of the loop
            result.RemoveAt(0);
            result.RemoveAt(result.Count - 1);
            foreach (var row in result)
            {
                foreach (var value in row)
                {
                    _logger.LogInformation(value);
                }
                TimeTrackingEntry temp = new TimeTrackingEntry(row[0], row[1], row[2], row[3], row[4], row[5]) { };
                TimeTrackingEntries.Add(temp);

            }
            return TimeTrackingEntries; // Returning result instead of TimeTrackingEntries
        }
    }
    public List<RequestObject> CreateRequestObjects(List<TimeTrackingEntry> TimeTrackingEntries)
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
                Date = DateTime.Parse(ConvertDateFormat(entry.Date)),
                IsLocked = false,
                Comment = entry.Comment,
                InternalComment = "",
                LastChanged = DateTime.Parse(ConvertDateFormat(entry.Date)),
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
    static string ConvertDateFormat(string inputDate)
    {
        // Parse the input date string
        DateTime originalDate = DateTime.ParseExact(inputDate, "M/d/yyyy h:mm:ss tt", null);

        // Convert it to the desired format
        string convertedDate = originalDate.ToString("yyyy-MM-dd");

        return convertedDate;
    }

}