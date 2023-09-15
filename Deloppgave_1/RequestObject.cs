using Azure.Core;

public class RequestObject
{
    public int Id { get; set; }
    public string ActivityCode { get; set; }
    public string ProjectCode { get; set; }
    public int EmployeeCode { get; set; }
    public int CustomerCode { get; set; }
    public string HourType { get; set; }
    public DateTime Date { get; set; }
    public bool IsLocked { get; set; }
    public string Comment { get; set; }
    public string InternalComment { get; set; }
    public DateTime LastChanged { get; set; }
    public bool ExcludedFromPayroll { get; set; }
    public bool IsTransferedToPayroll { get; set; }
    public bool IsInvoiced { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal HourlyCost { get; set; }
    public int Minutes { get; set; }
    public decimal BillableHours { get; set; }
    public decimal BillableAmount { get; set; }
    public int BreakTime { get; set; }

    // public RequestObject()
    // {
    //     Id = 0;
    //     ActivityCode = "100";
    //     ProjectCode = "2";
    //     EmployeeCode = 9;
    //     CustomerCode = 10000;
    //     HourType = "Overtid 100%";
    //     Date = DateTime.Parse("2023-02-27");
    //     IsLocked = false;
    //     Comment = "";
    //     InternalComment = "";
    //     LastChanged = DateTime.Parse("2023-03-03T09:35:03 +00:00");
    //     ExcludedFromPayroll = false;
    //     IsTransferedToPayroll = false;
    //     IsInvoiced = false;
    //     HourlyRate = 1250.0000m;
    //     HourlyCost = 0.0000m;
    //     Minutes = 9999999;
    //     BillableHours = 7.0000m;
    //     BillableAmount = 8750.0000m;
    //     BreakTime = 0;
    // }
}
